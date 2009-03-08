using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using Multitouch.Contracts;
using Multitouch.Service.Logic.Properties;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	class ConfigurationInterfaceService : IConfigurationInterface
	{
		readonly MultitouchInput input;

		public ConfigurationInterfaceService(MultitouchInput input)
		{
			this.input = input;
		}

		public InputProviderToken[] GetAvailableInputProviders()
		{
			var list = from token in input.InputProviders
					   select new InputProviderToken(token.MetadataView);
			return list.ToArray();
		}

		public void SetCurrentInputProvider(InputProviderToken value)
		{
			Settings.Default.CurrentProvider = value.Name;
			Settings.Default.Save();
		}

		public InputProviderToken GetCurrentInputProvider()
		{
			Export<IProvider, IAddInView> addInToken = (from token in input.InputProviders
									 where token.MetadataView.Id == Settings.Default.CurrentProvider
									 select token).FirstOrDefault();
			if(addInToken != null)
				return new InputProviderToken(addInToken.MetadataView);
			return null;
		}

		public void RestartService()
		{
			MultitouchInput multitouchInput = MultitouchInput.Instance;
			if(multitouchInput != null)
				multitouchInput.Restart();
		}

		public void ShowConfiguration(IntPtr parent)
		{
			MultitouchInput.Instance.ShowConfiguration(parent);
		}

		public bool HasConfiguration()
		{
			return MultitouchInput.Instance.HasConfiguration;
		}
	}
}
