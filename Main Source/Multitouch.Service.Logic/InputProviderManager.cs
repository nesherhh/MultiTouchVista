using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Multitouch.Contracts;
using Multitouch.Service.Logic.ExternalInterfaces;

namespace Multitouch.Service.Logic
{
	class InputProviderManager : IDisposable
	{
		IProvider inputProvider;
		ServiceHost serviceHost;

		static InputProviderManager instance;

		public InputProviderManager(IProvider inputProvider)
		{
			instance = this;

			StartService();

			this.inputProvider = inputProvider;
			this.inputProvider.Input += inputProvider_Input;
			this.inputProvider.Start();
		}

		public static InputProviderManager Instance
		{
			get { return instance; }
		}

		public IProvider Provider
		{
			get { return inputProvider; }
		}

		public void Dispose()
		{
            inputProvider.Stop();
			inputProvider.Input -= inputProvider_Input;
			inputProvider = null;

			serviceHost.Close();
		}

		void inputProvider_Input(object sender, InputDataEventArgs e)
		{
			ApplicationInterfaceService.DispatchInput(e);
		}

		void StartService()
		{
			Uri baseAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			serviceHost = new ServiceHost(typeof(ApplicationInterfaceService), baseAddress);
			NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			serviceHost.AddServiceEndpoint(typeof(IApplicationInterface), binding, string.Empty);

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