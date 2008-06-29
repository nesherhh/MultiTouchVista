using System;
using System.AddIn.Hosting;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Multitouch.Contracts;
using Multitouch.Service.Logic.ExternalInterfaces;
using Multitouch.Service.Logic.Properties;

namespace Multitouch.Service.Logic
{
	public class MultitouchInput
	{
		InputProviderManager providerManager;
		ServiceHost serviceHost;

		internal static MultitouchInput Instance { get; private set; }

		public MultitouchInput()
		{
			Instance = this;
			StartConfigurationService();
		}

		public void Start()
		{
			string provider = Settings.Default.CurrentProvider;
			if (!string.IsNullOrEmpty(provider))
			{
				AddInToken.EnableDirectConnect = true;

				string[] warnings = AddInStore.Update(PipelineStoreLocation.ApplicationBase);
				Array.ForEach(warnings, w => Trace.TraceWarning(w));

				Collection<AddInToken> providerTokens = AddInStore.FindAddIns(typeof(IProvider), PipelineStoreLocation.ApplicationBase);

				AddInToken currentProviderToken = (from token in providerTokens
				                                   where token.AddInFullName.Equals(provider)
				                                   select token).FirstOrDefault();
				if (currentProviderToken == null)
					throw new MultitouchException(string.Format("Input provider '{0}' could not be found", provider));

				IProvider activatedProvider = currentProviderToken.Activate<IProvider>(AppDomain.CurrentDomain);
				providerManager = new InputProviderManager(activatedProvider);
			}
		}

		public void Stop()
		{
			if (providerManager != null)
			{
				providerManager.Dispose();
				providerManager = null;
			}
		}

		public void Restart()
		{
			Stop();
			Start();
		}

		void StartConfigurationService()
		{
			Uri baseAddress = new Uri("net.pipe://localhost/Multitouch.Service/ConfigurationInterface");
			serviceHost = new ServiceHost(typeof(ConfigurationInterfaceService), baseAddress);
			NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			serviceHost.AddServiceEndpoint(typeof(IConfigurationInterface), binding, string.Empty);

			ServiceMetadataBehavior serviceMetadataBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (serviceMetadataBehavior == null)
				serviceMetadataBehavior = new ServiceMetadataBehavior();
			serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
			serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);

			serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
			serviceHost.Open();
		}
	}
}