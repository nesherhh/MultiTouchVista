using System;
using System.Diagnostics;
using System.ServiceModel;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	class ServiceCommunicator : IDisposable
	{
		CommunicationLogic logic;
		ApplicationInterfaceClient service;
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
			GetService().RemoveWindowHandleFromSession(handle);
		}

		internal void CreateSession()
		{
			Uri serviceAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			EndpointAddress remoteAddress = new EndpointAddress(serviceAddress);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			namedPipeBinding.MaxReceivedMessageSize = int.MaxValue;
			namedPipeBinding.MaxBufferSize = int.MaxValue;
			namedPipeBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;

			IApplicationInterfaceCallback dispatcher = new MultitouchServiceContactDispatcher(logic);
			InstanceContext instanceContext = new InstanceContext(dispatcher);
			service = new ApplicationInterfaceClient(instanceContext, namedPipeBinding, remoteAddress);

			try
			{
				service.CreateSession();
				contactDispatcher = dispatcher;
			}
			catch (EndpointNotFoundException e)
			{
				throw new MultitouchException("Could not connect to Multitouch service, please start Multitouch input server before running this application.", e);
			}
		}

		internal void RemoveSession()
		{
			if (contactDispatcher != null)
			{
				try
				{
					service.RemoveSession();
					service.Close();
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

		ApplicationInterfaceClient GetService()
		{
			if (service == null)
				throw new MultitouchException("No connection to Multitouch service");
			return service;
		}
	}
}