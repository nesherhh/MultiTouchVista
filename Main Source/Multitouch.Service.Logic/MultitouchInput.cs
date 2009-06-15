using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using Multitouch.Contracts;
using Multitouch.Service.Logic.ExternalInterfaces;
using Multitouch.Service.Logic.MEF;
using Multitouch.Service.Logic.Properties;

namespace Multitouch.Service.Logic
{
	public class MultitouchInput
	{
		internal static MultitouchInput Instance { get; private set; }

		ServiceHost serviceHost;
        internal InputProviderManager ProviderManager { get; private set; }

		public bool HasConfiguration
		{
			get { return ProviderManager.Provider.HasConfiguration; }
		}

		[Import(typeof(IProvider))]
		public ExportCollection<IProvider, IAddInView> InputProviders { get; set; }

		public MultitouchInput()
		{
			Combine();

			Instance = this;
			StartConfigurationService();
		}

		void Combine()
		{
			MultipleDirectoryCatalog catalog = new MultipleDirectoryCatalog("AddIns", false, "*.dll");
			CompositionContainer container = new CompositionContainer(catalog);
			CompositionBatch batch = new CompositionBatch();
			batch.AddPart(this);
			container.Compose(batch);
		}

		public void Start()
		{
			string providerId = Settings.Default.CurrentProvider;

			Console.WriteLine("Input provider in settings - " + providerId);

			if (!string.IsNullOrEmpty(providerId))
			{
				Export<IProvider, IAddInView> currentProvider = (from provider in InputProviders
																 where provider.MetadataView.Id == providerId
																 select provider).FirstOrDefault();
				if (currentProvider == null)
					throw new MultitouchException(string.Format("Input provider '{0}' could not be found", providerId));

				string text = "Found:" + Environment.NewLine +
							  "Name: " + currentProvider.MetadataView.Id + Environment.NewLine +
							  "Description: " + currentProvider.MetadataView.Description + Environment.NewLine +
							  "Publisher: " + currentProvider.MetadataView.Publisher + Environment.NewLine +
							  "Version: " + currentProvider.MetadataView.Version;
				Console.WriteLine(text);

				ProviderManager = new InputProviderManager(currentProvider.GetExportedObject());
			}
		}

		public void Stop()
		{
			if (ProviderManager != null)
			{
				ProviderManager.Dispose();
				ProviderManager = null;
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
			serviceHost = new ServiceHost(new ConfigurationInterfaceService(this), baseAddress);
			NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			binding.ReceiveTimeout = TimeSpan.MaxValue;
			binding.MaxBufferSize = int.MaxValue;
			binding.MaxReceivedMessageSize = int.MaxValue;
			serviceHost.AddServiceEndpoint(typeof(IConfigurationInterface), binding, string.Empty);

			ServiceMetadataBehavior serviceMetadataBehavior = serviceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
			if (serviceMetadataBehavior == null)
				serviceMetadataBehavior = new ServiceMetadataBehavior();
			serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
			serviceHost.Description.Behaviors.Add(serviceMetadataBehavior);

			serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
			serviceHost.Open();
		}

		public void ShowConfiguration(IntPtr parent)
		{
			Thread t = new Thread(() =>
								  {
									  UIElement configuration = ProviderManager.Provider.GetConfiguration();
									  Window window = configuration as Window;
									  if (window == null)
									  {
										  window = new Window();
										  window.Content = configuration;
									  }
									  else
									  {
										  WindowInteropHelper helper = new WindowInteropHelper(window);
										  helper.Owner = parent;
										  window.ShowDialog();
									  }
								  });
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
		}
	}
}