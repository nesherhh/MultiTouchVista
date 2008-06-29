using System;
using System.Diagnostics;
using System.ServiceModel;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	class ServiceCommunicator
	{
		CommunicationLogic logic;
		ApplicationInterfaceClient client;
		IApplicationInterfaceCallback contactDispatcher;

		public ServiceCommunicator(CommunicationLogic logic)
		{
			this.logic = logic;
		}

		public void Connect(IntPtr windowHandle)
		{
			Uri serviceAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			EndpointAddress remoteAddress = new EndpointAddress(serviceAddress);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

			IApplicationInterfaceCallback dispatcher = new MultitouchServiceContactDispatcher(logic);
			InstanceContext instanceContext = new InstanceContext(dispatcher);
			client = new ApplicationInterfaceClient(instanceContext, namedPipeBinding, remoteAddress);

			try
			{
				client.Subscribe(windowHandle);
				contactDispatcher = dispatcher;
			}
			catch (EndpointNotFoundException)
			{
				Trace.TraceWarning("Could not connect to Multitouch service, falling back to single touch mouse input");
				contactDispatcher = new SingleMouseContactDispatcher(logic);
			}
		}

		public void Disconnect()
		{
			if (contactDispatcher != null)
			{
				client.Unsubscribe();
				client.Close();
				contactDispatcher = null;
				client = null;
			}
		}
	}
}