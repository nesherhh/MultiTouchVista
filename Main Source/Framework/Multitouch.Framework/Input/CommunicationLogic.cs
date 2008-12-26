using System;
using System.Collections.Generic;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// Logic that communicates with Multitouch service.
	/// </summary>
	public class CommunicationLogic
	{
		static CommunicationLogic instance;
		static object lockInstance = new object();
		ServiceCommunicator serviceCommunicator;
		List<IContactHandler> contactHandlers;
		
		public event EventHandler<FrameEventArgs> Frame;

		CommunicationLogic()
		{
			serviceCommunicator = new ServiceCommunicator(this);
			contactHandlers = new List<IContactHandler>();
		}

		/// <summary>
		/// Gets the default instance of logic (Singleton implementation).
		/// </summary>
		/// <value>The instance.</value>
		public static CommunicationLogic Instance
		{
			get
			{
				if (instance == null)
				{
					lock (lockInstance)
					{
						if (instance == null)
							instance = new CommunicationLogic();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether an event will be raised on every frame.
		/// </summary>
		/// <value><c>true</c> if [enable frame event]; otherwise, <c>false</c>.</value>
		public bool EnableFrameEvent
		{
			get { return serviceCommunicator.EnableFrameEvent; }
			set { serviceCommunicator.EnableFrameEvent = value; }
		}

		/// <summary>
		/// Makes a connection to Multitouch service.
		/// </summary>
		/// <param name="windowHandle">The Handle of a window of your application. This handle used to send multitouch events to this window.</param>
		public void Connect(IntPtr windowHandle)
		{
			if (contactHandlers.Count == 1)
				serviceCommunicator.Connect(windowHandle);
		}

		/// <summary>
		/// Disconnects from Multitouch service. You should run this method before closing your application.
		/// </summary>
		public void Disconnect()
		{
			if (contactHandlers.Count == 0)
				serviceCommunicator.Disconnect();
		}

		/// <summary>
		/// Registers the contact handler. All contacts received from service will be send to this handler for processing in your application.
		/// </summary>
		/// <param name="contactHandler">Instance of an object that implementats <see cref="IContactHandler"/> interface.</param>
		public void RegisterContactHandler(IContactHandler contactHandler)
		{
			contactHandlers.Add(contactHandler);
		}

		/// <summary>
		/// Unregisters contact handler.
		/// </summary>
		/// <param name="contactHandler">Instance of an object that implementats <see cref="IContactHandler"/> interface.</param>
		public void UnregisterContactHandler(IContactHandler contactHandler)
		{
			contactHandlers.Remove(contactHandler);
		}
        
		internal void ProcessChangedContact(int id, double x, double y, double width, double height, Service.ContactState state)
		{
			ContactState contactState;
			switch (state)
			{
				case Service.ContactState.New:
					contactState = ContactState.New;
					break;
				case Service.ContactState.Removed:
					contactState = ContactState.Removed;
					break;
				case Service.ContactState.Moved:
					contactState = ContactState.Moved;
					break;
				default:
					throw new ArgumentOutOfRangeException("state");
			}

			foreach (IContactHandler handler in contactHandlers)
				handler.ProcessContactChange(id, x, y, width, height, contactState);
		}

		internal void ProcessFrame(Service.FrameData data)
		{
			EventHandler<FrameEventArgs> eventHandler = Frame;
			if(eventHandler != null)
				eventHandler(null, new FrameEventArgs(data));
		}

		/// <summary>
		/// Shoulds receive image from camera.
		/// </summary>
		/// <param name="imageType">Type of the image.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public bool ShouldReceiveImage(ImageType imageType, bool value)
		{
			Service.ImageType type;
			switch (imageType)
			{
				case ImageType.Normalized:
					type = Service.ImageType.Normalized;
					break;
				default:
					throw new ArgumentOutOfRangeException("imageType");
			}
			return serviceCommunicator.ShouldReceiveImage(type, value);
		}
	}
}