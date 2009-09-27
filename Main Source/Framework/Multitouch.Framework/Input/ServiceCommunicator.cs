using System;
using System.Diagnostics;
using System.ServiceModel;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	class ServiceCommunicator : IDisposable
	{
		readonly CommunicationLogic logic;
		IApplicationInterface service;
		IApplicationInterfaceCallback contactDispatcher;
		bool sendEmptyFrames;

		public ServiceCommunicator(CommunicationLogic logic)
		{
			this.logic = logic;
		}

		public void AddWindowToSession(IntPtr handle)
		{
			GetService().AddWindowHandleToSession(handle);
		}

		public void RemoveWindowFromSession(IntPtr handle)
		{
			try
			{
				GetService().RemoveWindowHandleFromSession(handle);
			}
			catch (CommunicationException)
			{ }
		}

		internal void CreateSession()
		{
			Uri serviceAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			EndpointAddress remoteAddress = new EndpointAddress(serviceAddress);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			namedPipeBinding.MaxReceivedMessageSize = int.MaxValue;
			namedPipeBinding.MaxBufferSize = int.MaxValue;
			namedPipeBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
			namedPipeBinding.ReceiveTimeout = TimeSpan.MaxValue;

			IApplicationInterfaceCallback dispatcher = new MultitouchServiceContactDispatcher(logic);
			InstanceContext instanceContext = new InstanceContext(dispatcher);
			service = new ApplicationInterfaceClient(instanceContext, namedPipeBinding, remoteAddress);

			try
			{
				service.CreateSession();
				MouseHelper.SingleMouseFallback = false;
			}
			catch (EndpointNotFoundException)
			{
				//throw new MultitouchException("Could not connect to Multitouch service, please start Multitouch input server before running this application.", e);
				Trace.TraceWarning("Could not connect to Multitouch service. Enabling single mouse input.");
				SingleMouseClientAndDispatcher client = new SingleMouseClientAndDispatcher(logic);
				service = client;
				dispatcher = client;
				MouseHelper.SingleMouseFallback = true;
			}
			contactDispatcher = dispatcher;
		}

		internal void RemoveSession()
		{
			if (contactDispatcher != null)
			{
				try
				{
					service.RemoveSession();
					IDisposable client = service as IDisposable;
					if(client != null)
						client.Dispose();
				}
				catch (Exception e)
				{
					Trace.TraceWarning(e.ToString());
				}
				contactDispatcher = null;
				service = null;
			}
		}

		public bool SendImageType(Service.ImageType imageType, bool value)
		{
			return GetService().SendImageType(imageType, value);
		}

		public bool SendEmptyFrames
		{
			get { return sendEmptyFrames; }
			set
			{
				sendEmptyFrames = value;
				GetService().SendEmptyFrames(value);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				RemoveSession();
		}

		IApplicationInterface GetService()
		{
			if (service == null)
				throw new MultitouchException("No connection to Multitouch service");
			return service;
		}
	}
}