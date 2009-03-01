using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
	public class ApplicationInterfaceService : IApplicationInterface
	{
		readonly Dictionary<string, SessionContext> sessions;
		readonly Dictionary<IntPtr, SessionContext> handleToSessionMap;

		public ApplicationInterfaceService()
		{
			sessions = new Dictionary<string, SessionContext>();
			handleToSessionMap = new Dictionary<IntPtr, SessionContext>();
		}

		public void CreateSession()
		{
			string sessionId = OperationContext.Current.SessionId;
			if (sessions.ContainsKey(sessionId))
				throw new MultitouchException(string.Format("session '{0}' is already created", sessionId));
			sessions.Add(sessionId, new SessionContext(sessionId, OperationContext.Current.GetCallbackChannel<IApplicationInterfaceCallback>()));

			UpdateGlobalSendEmptyFrames();
			UpdateGlobalImageSetting(ImageType.Binarized);
			UpdateGlobalImageSetting(ImageType.Normalized);
		}

		public void RemoveSession()
		{
			RemoveSession(GetSessionContext());
		}

		void RemoveSession(SessionContext session)
		{
			foreach (IntPtr handle in session)
				handleToSessionMap.Remove(handle);
			if (OperationContext.Current != null)
				sessions.Remove(OperationContext.Current.SessionId);
			else
				sessions.Remove(session.SessionId);

			UpdateGlobalSendEmptyFrames();
			UpdateGlobalImageSetting(ImageType.Binarized);
			UpdateGlobalImageSetting(ImageType.Normalized);
		}

		public void AddWindowHandleToSession(IntPtr windowHandle)
		{
			SessionContext sessionContext = GetSessionContext();
			sessionContext.Add(windowHandle);
			handleToSessionMap.Add(windowHandle, sessionContext);
		}

		public void RemoveWindowHandleFromSession(IntPtr windowHandle)
		{
			GetSessionContext().Remove(windowHandle);
			handleToSessionMap.Remove(windowHandle);
		}

		SessionContext GetSessionContext()
		{
			string sessionId = OperationContext.Current.SessionId;
			SessionContext sessionContext;
			if (!sessions.TryGetValue(sessionId, out sessionContext))
				throw new MultitouchException(string.Format("Session '{0}' is not registered. You have to execute CreateSession first", sessionId));
			return sessionContext;
		}

		public void SendEmptyFrames(bool value)
		{
			GetSessionContext().SendEmptyFrames = value;
			UpdateGlobalSendEmptyFrames();
		}

		public bool SendImageType(ImageType imageType, bool value)
		{
			SessionContext sessionContext = GetSessionContext();
			sessionContext.ImagesToSend[imageType] = true;
			return sessionContext.ImagesToSend[imageType] = UpdateGlobalImageSetting(imageType);
		}

		void UpdateGlobalSendEmptyFrames()
		{
			bool sendEmptyFrames = false;
			foreach (SessionContext context in sessions.Values)
				sendEmptyFrames = sendEmptyFrames || context.SendEmptyFrames;
			InputProviderManager.Instance.Provider.SendEmptyFrames = sendEmptyFrames;
		}

		bool UpdateGlobalImageSetting(ImageType imageType)
		{
			int count = sessions.Count(app => app.Value.ImagesToSend[imageType]);
			return InputProviderManager.Instance.Provider.SendImageType(imageType, count > 0);
		}

		public void DispatchFrame(NewFrameEventArgs e)
		{
			if(sessions.Count == 0)
				return;

			// For each contact determinte it's target handle and group by this handle
			IEnumerable<IGrouping<IntPtr, IContactData>> contactsGroups = e.Contacts.GroupBy(contact => Utils.GetWindowFromPoint(contact.Position));

			// Create a list with sessions and contacts that belong to this session
			Dictionary<SessionContext, List<ContactData>> sessionList = new Dictionary<SessionContext, List<ContactData>>();

			IntPtr invalidHandle = new IntPtr(-1);
			foreach (IGrouping<IntPtr, IContactData> contactsGroup in contactsGroups.Where(g => !g.Key.Equals(invalidHandle)))
			{
				List<ContactData> contacts;

				SessionContext sessionContext;
				if (!handleToSessionMap.TryGetValue(contactsGroup.Key, out sessionContext))
				{
					if (!handleToSessionMap.TryGetValue(IntPtr.Zero, out sessionContext))
						continue;
				}

				if(!sessionList.TryGetValue(sessionContext, out contacts))
				{
					contacts = new List<ContactData>();
					sessionList.Add(sessionContext, contacts);
				}

				contacts.AddRange(contactsGroup.Select(c => new ContactData(c, contactsGroup.Key)));
			}

			List<SessionContext> emptyFrameReceivers = new List<SessionContext>(sessions.Values);

			// Send contact data to session
			foreach (KeyValuePair<SessionContext, List<ContactData>> keyValuePair in sessionList)
			{
				SessionContext session = keyValuePair.Key;
				try
				{
					session.Callback.Frame(new FrameData(e.Timestamp, keyValuePair.Value, GetImages(session, e.Images)));
				}
				catch (Exception exc)
				{
					Trace.TraceError("Error during sending frame to application:\r\n" + exc.Message);
					RemoveSession(session);
				}
				emptyFrameReceivers.Remove(keyValuePair.Key);
			}

			// Send frame data to sessions that requested empty frames
			foreach (SessionContext session in emptyFrameReceivers.Where(s => s.SendEmptyFrames))
			{
				try
				{
					session.Callback.Frame(new FrameData(e.Timestamp, new ContactData[0], GetImages(session, e.Images)));
				}
				catch (Exception exc)
				{
					Trace.TraceError("Error during sending frame to application:\r\n" + exc.Message);
					RemoveSession(session);
				}
			}
		}

		static IEnumerable<IImageData> GetImages(SessionContext context, IEnumerable<IImageData> availableImages)
		{
			IEnumerable<ImageType> imageTypes = context.ImagesToSend.Where(i => i.Value).Select(i => i.Key);
			return availableImages.Where(i => imageTypes.Contains(i.Type));
		}
	}
}