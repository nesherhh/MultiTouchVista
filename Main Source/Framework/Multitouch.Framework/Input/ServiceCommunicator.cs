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
		bool enableFrameEvent;

		public ServiceCommunicator(CommunicationLogic logic)
		{
			this.logic = logic;
		}

		public bool EnableFrameEvent
		{
			get { return enableFrameEvent; }
			set
			{
				if (client == null)
					throw new MultitouchException("Not connected");

				enableFrameEvent = value;
				client.ReceiveFrames(value);
			}
		}

		public void Connect(IntPtr windowHandle)
		{
			Uri serviceAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			EndpointAddress remoteAddress = new EndpointAddress(serviceAddress);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			namedPipeBinding.MaxReceivedMessageSize = int.MaxValue;
			namedPipeBinding.MaxBufferSize = int.MaxValue;
			namedPipeBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;

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

		public bool ShouldReceiveImage(Service.ImageType imageType, bool value)
		{
			if (client == null)
				throw new MultitouchException("Not connected");

			return client.SendImageType(imageType, value);
		}
	}
}