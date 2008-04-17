using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Logic.Service;

namespace Danilins.Multitouch.Logic
{
	public class ServiceLogic : Common.Logics.Logic
	{
		private MultitouchService service;
		private ServiceHost host;

		public ServiceLogic(IServiceProvider parentProvider)
			: base(parentProvider)
		{ }

		public void Start()
		{
			InputProviderLogic providerLogic = GetLogic<InputProviderLogic>();
			if (!providerLogic.IsRunning)
				throw new MultitouchException("No provider is currently running");

			service = new MultitouchService(providerLogic.CurrentProvider, GetLogic<LegacySupportLogic>());
			
			Uri baseAddress = new Uri("net.pipe://localhost/Danilins.Multitouch.Logic.Service/MultitouchService");
			host = new ServiceHost(service, baseAddress);

			NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			host.AddServiceEndpoint(typeof(IMultitouchService), binding, string.Empty);

			ServiceMetadataBehavior serviceMetadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (serviceMetadataBehavior == null)
				serviceMetadataBehavior = new ServiceMetadataBehavior();
			serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
			host.Description.Behaviors.Add(serviceMetadataBehavior);

			host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");

			host.Open();
		}

		public void Stop()
		{
			if (host == null)
				return;

			host.Close();
			service.Dispose();
		}

		public CommunicationState State
		{
			get
			{
				if (host == null)
					return CommunicationState.Closed;
				return host.State;
			}
		}
	}
}