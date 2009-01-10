using System;
using System.Collections.Generic;
using System.Linq;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// Logic that communicates with Multitouch service.
	/// </summary>
	class CommunicationLogic : IDisposable
	{
		ServiceCommunicator serviceCommunicator;
		Dictionary<IntPtr, HashSet<ContactHandler>> windowsMap;
		static object lockObject = new object();
		static CommunicationLogic instance;
		int receiveEmptyFramesCount;
		Dictionary<Service.ImageType, int> sendImagesCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommunicationLogic"/> class.
		/// </summary>
		public CommunicationLogic()
		{
			windowsMap = new Dictionary<IntPtr, HashSet<ContactHandler>>();
			sendImagesCount = new Dictionary<Service.ImageType, int>();
			foreach (Service.ImageType imageType in Enum.GetValues(typeof(Service.ImageType)))
				sendImagesCount[imageType] = 0;

			serviceCommunicator = new ServiceCommunicator(this);
		}

		/// <summary>
		/// Gets the instance of communication logic.
		/// </summary>
		/// <value>The instance.</value>
		public static CommunicationLogic Instance
		{
			get
			{
				if(instance == null)
				{
					lock(lockObject)
					{
						if(instance == null)
							instance = new CommunicationLogic();
					}
				}
				return instance;
			}
		}

		public void RegisterHandler(ContactHandler handler)
		{
			HashSet<ContactHandler> contactHandlers;
			if (windowsMap.TryGetValue(handler.Handle, out contactHandlers))
				contactHandlers.Add(handler);
			else
			{
				if(windowsMap.Count == 0)
					serviceCommunicator.CreateSession();
				contactHandlers = new HashSet<ContactHandler>();
				contactHandlers.Add(handler);
				windowsMap.Add(handler.Handle, contactHandlers);
				serviceCommunicator.AddWindowToSession(handler.Handle);
			}
		}

		public void UnregisterHandler(ContactHandler handler)
		{
			HashSet<ContactHandler> contactHandlers;
			if(windowsMap.TryGetValue(handler.Handle, out contactHandlers))
			{
				contactHandlers.Remove(handler);
				serviceCommunicator.RemoveWindowFromSession(handler.Handle);
				if (contactHandlers.Count == 0)
					windowsMap.Remove(handler.Handle);
				if(windowsMap.Count == 0)
					serviceCommunicator.RemoveSession();
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				serviceCommunicator.Dispose();
		}

		internal void DispatchFrame(FrameData data)
		{
			foreach (ContactData contact in data.Contacts)
				HandleContact(contact, data.Timestamp);
			HandleFrame(data);
		}

		void HandleContact(ContactData contact, long timestamp)
		{
			KeyValuePair<IntPtr, HashSet<ContactHandler>> first = windowsMap.FirstOrDefault(pair => pair.Key.Equals(contact.Hwnd));
			if (!Equals(first, default(KeyValuePair<IntPtr, HashSet<ContactHandler>>)))
			{
				foreach (ContactHandler handler in first.Value)
					handler.HandleContact(contact, timestamp);
			}
		}

		void HandleFrame(FrameData data)
		{
			IEnumerable<ContactHandler> handlers = windowsMap.SelectMany(pair => pair.Value.Where(handler => handler.ReceiveEmptyFrames
			                                                                                                 || data.Contacts.Count(c => c.Hwnd.Equals(pair.Key)) > 0));
			foreach (ContactHandler handler in handlers)
				handler.HandleFrame(data);
		}

		public void ReceiveEmptyFrames(bool value)
		{
			if (value)
				receiveEmptyFramesCount++;
			else
				receiveEmptyFramesCount--;
			serviceCommunicator.SendEmptyFrames = receiveEmptyFramesCount > 0;
		}

		public bool SendImageType(Service.ImageType imageType, bool value)
		{
			if (value)
				sendImagesCount[imageType]++;
			else
				sendImagesCount[imageType]--;

			bool willBeSend = serviceCommunicator.SendImageType(imageType, sendImagesCount[imageType] > 0);
			if (!willBeSend)
				sendImagesCount[imageType] = 0;
			return willBeSend;
		}
	}
}