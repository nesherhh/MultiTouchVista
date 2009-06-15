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
		ApplicationInterfaceService applicationService;

		public InputProviderManager(IProvider inputProvider)
		{
			instance = this;

			StartService();

			this.inputProvider = inputProvider;
			this.inputProvider.NewFrame += inputProvider_NewFrame;
			this.inputProvider.Start();
			Console.WriteLine("Started input provider");
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
			inputProvider.NewFrame -= inputProvider_NewFrame;
			IDisposable disposable = inputProvider as IDisposable;
			if(disposable != null)
				disposable.Dispose();
			inputProvider = null;

			Console.WriteLine("Stopped input provider");

			serviceHost.Close();
		}

		void inputProvider_NewFrame(object sender, NewFrameEventArgs e)
		{
			applicationService.DispatchFrame(e);
		}

		void StartService()
		{
			applicationService = new ApplicationInterfaceService();

			Uri baseAddress = new Uri("net.pipe://localhost/Multitouch.Service/ApplicationInterface");
			serviceHost = new ServiceHost(applicationService, baseAddress);
			NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			binding.ReceiveTimeout = TimeSpan.MaxValue;
			binding.MaxBufferSize = int.MaxValue;
			binding.MaxReceivedMessageSize = int.MaxValue;
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