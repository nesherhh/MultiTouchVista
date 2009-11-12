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
		
		//several sessions can receive data for the same handle.
		readonly Dictionary<SessionContext, ICollection<IntPtr>> sessionToHandlesMap;

		public ApplicationInterfaceService()
		{
			sessions = new Dictionary<string, SessionContext>();
			sessionToHandlesMap = new Dictionary<SessionContext, ICollection<IntPtr>>();
		}

		public void CreateSession()
		{
			string sessionId = OperationContext.Current.SessionId;
			if (sessions.ContainsKey(sessionId))
				throw new MultitouchException(string.Format("session '{0}' is already created", sessionId));
			SessionContext context = new SessionContext(sessionId, OperationContext.Current.GetCallbackChannel<IApplicationInterfaceCallback>());
			sessions.Add(sessionId, context);
			sessionToHandlesMap.Add(context, new HashSet<IntPtr>());

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
			sessionToHandlesMap.Remove(session);
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

			ICollection<IntPtr> handlesList;
			if(sessionToHandlesMap.TryGetValue(sessionContext, out handlesList))
				handlesList.Add(windowHandle);
		}

		public void RemoveWindowHandleFromSession(IntPtr windowHandle)
		{
			SessionContext context = GetSessionContext();
			context.Remove(windowHandle);

			ICollection<IntPtr> handlesList;
			if (sessionToHandlesMap.TryGetValue(context, out handlesList))
				handlesList.Remove(windowHandle);
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
			IEnumerable<IGrouping<IntPtr, Contact>> contactsGroupedByHandle = e.Contacts.GroupBy(contact => Utils.GetWindowFromPoint(contact.Position));


			// Create a list with sessions and contacts that belong to this session
			Dictionary<SessionContext, List<ContactData>> sessionList = new Dictionary<SessionContext, List<ContactData>>();

			foreach (IGrouping<IntPtr, Contact> handleWithContacts in contactsGroupedByHandle)
			{
				List<ContactData> contacts;

				foreach (SessionContext sessionContext in from map in sessionToHandlesMap
														  where map.Value.Contains(handleWithContacts.Key)
														  || map.Value.Contains(IntPtr.Zero)
														  select map.Key)
				{
					if (!sessionList.TryGetValue(sessionContext, out contacts))
					{
						contacts = new List<ContactData>();
						sessionList.Add(sessionContext, contacts);
					}
					contacts.AddRange(handleWithContacts.Select(c => new ContactData(c, handleWithContacts.Key)));
				}
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
					session.Callback.Frame(new FrameData(e.Timestamp, Enumerable.Empty<ContactData>(), GetImages(session, e.Images)));
				}
				catch (Exception exc)
				{
					Trace.TraceError("Error during sending frame to application:\r\n" + exc.Message);
					RemoveSession(session);
				}
			}
		}

		static IEnumerable<ImageData> GetImages(SessionContext context, IEnumerable<Image> availableImages)
		{
			if (availableImages == null)
				return Enumerable.Empty<ImageData>();
			IEnumerable<ImageType> imageTypes = context.ImagesToSend.Where(i => i.Value).Select(i => i.Key);
			return availableImages.Where(i => imageTypes.Contains(i.Type)).Select(i => new ImageData(i));
		}
	}
}