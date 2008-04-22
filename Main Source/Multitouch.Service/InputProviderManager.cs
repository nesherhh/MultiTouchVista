using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Multitouch.Contracts;
using Multitouch.Service.ExternalInterfaces;

namespace Multitouch.Service
{
	class InputProviderManager : IDisposable
	{
		IProvider inputProvider;
		ServiceHost serviceHost;

		public InputProviderManager(IProvider inputProvider)
		{
			this.inputProvider = inputProvider;
			this.inputProvider.ContactChanged += inputProvider_ContactChanged;
			this.inputProvider.Start();

			StartService();
		}

		public void Dispose()
		{
			serviceHost.Close();

			inputProvider.Stop();
			inputProvider.ContactChanged -= inputProvider_ContactChanged;
			inputProvider = null;
		}

		void inputProvider_ContactChanged(object sender, ContactChangedEventArgs e)
		{
			IContact contact = e.Contact;
			string contactString = string.Format("ID: {0}, XY: {1};{2} W:{3}, H:{4}, State: {5}", contact.Id, contact.X, contact.Y, contact.Width,
				contact.Height, contact.State);
			Console.WriteLine(contactString);

			ApplicationInterfaceService.ContactChanged(e);
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