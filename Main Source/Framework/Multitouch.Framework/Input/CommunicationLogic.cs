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
		List<IContactHandler> handlers;

		CommunicationLogic()
		{
			serviceCommunicator = new ServiceCommunicator(this);
			handlers = new List<IContactHandler>();
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
		/// Makes a connection to Multitouch service.
		/// </summary>
		/// <param name="windowHandle">The Handle of a window of your application. This handle used to send multitouch events to this window.</param>
		public void Connect(IntPtr windowHandle)
		{
			if (handlers.Count == 1)
				serviceCommunicator.Connect(windowHandle);
		}

		/// <summary>
		/// Disconnects from Multitouch service. You should run this method before closing your application.
		/// </summary>
		public void Disconnect()
		{
			if (handlers.Count == 0)
				serviceCommunicator.Disconnect();
		}

		/// <summary>
		/// Registers the contact handler. All contacts received from service will be send to this handler for processing in your application.
		/// </summary>
		/// <param name="contactHandler">Instance of an object that implementats <see cref="IContactHandler"/> interface.</param>
		public void RegisterContactHandler(IContactHandler contactHandler)
		{
			handlers.Add(contactHandler);
		}

		/// <summary>
		/// Unregisters contact handler.
		/// </summary>
		/// <param name="contactHandler">Instance of an object that implementats <see cref="IContactHandler"/> interface.</param>
		public void UnregisterContactHandler(IContactHandler contactHandler)
		{
			handlers.Remove(contactHandler);
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

			foreach (IContactHandler handler in handlers)
				handler.ProcessContactChange(id, x, y, width, height, contactState);
		}
	}
}