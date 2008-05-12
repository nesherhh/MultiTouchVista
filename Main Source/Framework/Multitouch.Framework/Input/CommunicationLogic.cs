using System;
using System.Collections.Generic;

namespace Multitouch.Framework.Input
{
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

		public void Connect(IntPtr windowHandle)
		{
			if(handlers.Count == 1)
				serviceCommunicator.Connect(windowHandle);
		}

		public void Disconnect()
		{
			if(handlers.Count == 0)
				serviceCommunicator.Disconnect();
		}

		public void RegisterInputProvider(IContactHandler contactHandler)
		{
			handlers.Add(contactHandler);
		}

		public void UnRegisterInputProvider(IContactHandler contactHandler)
		{
			handlers.Remove(contactHandler);
		}

		internal void ProcessChangedContact(int id, double x, double y, double width, double height, ContactState state)
		{
			foreach (IContactHandler handler in handlers)
				handler.ProcessContactChange(id, x, y, width, height, state);
		}
	}
}
