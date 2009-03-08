using System;
using System.Linq;
using System.ServiceModel;
using Multitouch.Configuration.Service;

namespace Multitouch.Configuration
{
	/// <summary>
	/// Configuration logic.
	/// </summary>
	public class ConfigurationLogic
	{
		static ConfigurationInterfaceClient configurationService;

		/// <summary>
		/// Gets or sets current input provider.
		/// </summary>
		public InputProvider CurrentProvider
		{
			get
			{
				InputProviderToken provider = ConfigurationService.GetCurrentInputProvider();
				if (provider != null)
					return new InputProvider(provider);
				return null;
			}
			set
			{
				if (value != null)
					ConfigurationService.SetCurrentInputProvider(value.Token);
			}
		}

		ConfigurationInterfaceClient ConfigurationService
		{
			get
			{
				if (configurationService == null)
					CreateService();
				return configurationService;
			}
		}

		void CreateService()
		{
			Uri configurationServiceAddress = new Uri("net.pipe://localhost/Multitouch.Service/ConfigurationInterface");
			EndpointAddress remoteAddress = new EndpointAddress(configurationServiceAddress);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			configurationService = new ConfigurationInterfaceClient(namedPipeBinding, remoteAddress);
			try
			{
				configurationService.GetCurrentInputProvider();
			}
			catch (EndpointNotFoundException e)
			{
				throw new ServiceErrorException("Multitouch service is not running. Please start multitouch service before trying to configure it.", e);
			}
		}

		/// <summary>
		/// Returns all available input providers.
		/// </summary>
		/// <returns></returns>
		public InputProvider[] AvailableProviders
		{
			get { return ConfigurationService.GetAvailableInputProviders().Select(t => new InputProvider(t)).ToArray(); }
		}

		/// <summary>
		/// Restarts multitouch service.
		/// </summary>
		public void RestartService()
		{
			ConfigurationService.RestartService();
		}

		/// <summary>
		/// Gets a value indicating whether this instance has configuration.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has configuration; otherwise, <c>false</c>.
		/// </value>
		public bool HasConfiguration
		{
			get
			{
				if (CurrentProvider != null)
					return ConfigurationService.HasConfiguration();
				return false;
			}
		}

		/// <summary>
		/// Shows the configuration.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public void ShowConfiguration(IntPtr parent)
		{
			ConfigurationService.ShowConfiguration(parent);
		}
	}
}