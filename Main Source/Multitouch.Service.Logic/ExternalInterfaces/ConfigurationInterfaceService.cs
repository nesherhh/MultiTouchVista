using System;
using System.AddIn.Hosting;
using System.Linq;
using System.ServiceModel;
using Multitouch.Contracts;
using Multitouch.Service.Logic.Properties;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	class ConfigurationInterfaceService : IConfigurationInterface
	{
		public InputProviderToken[] GetAvailableInputProviders()
		{
			var list = from token in AddInStore.FindAddIns(typeof(IProvider), PipelineStoreLocation.ApplicationBase)
					   select new InputProviderToken(token);
			return list.ToArray();
		}

		public void SetCurrentInputProvider(InputProviderToken value)
		{
			Settings.Default.CurrentProvider = value.AddInFullName;
		}

		public InputProviderToken GetCurrentInputProvider()
		{
			AddInToken addInToken = (from token in AddInStore.FindAddIns(typeof(IProvider), PipelineStoreLocation.ApplicationBase)
			                        where token.AddInFullName.Equals(Settings.Default.CurrentProvider)
			                        select token).FirstOrDefault();
			if(addInToken != null)
				return new InputProviderToken(addInToken);
			return null;
		}

		public void RestartService()
		{
			MultitouchInput multitouchInput = MultitouchInput.Instance;
			if(multitouchInput != null)
				multitouchInput.Restart();
		}
	}
}
