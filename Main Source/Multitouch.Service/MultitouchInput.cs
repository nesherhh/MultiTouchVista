using System;
using System.AddIn.Hosting;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Multitouch.Contracts;
using Multitouch.Service.Properties;

namespace Multitouch.Service
{
	class MultitouchInput
	{
		InputProviderManager providerManager;

		public void Start()
		{
			AddInToken.EnableDirectConnect = true;

			string[] warnings = AddInStore.Update(PipelineStoreLocation.ApplicationBase);
			Array.ForEach(warnings, w => Trace.TraceWarning(w));

			Collection<AddInToken> providerTokens = AddInStore.FindAddIns(typeof(IProvider), PipelineStoreLocation.ApplicationBase);

			string provider = Settings.Default.CurrentProvider;
			AddInToken currentProviderToken = (from token in providerTokens
											   where token.AddInFullName.Equals(provider)
											   select token).FirstOrDefault();
			if (currentProviderToken == null)
				throw new MultitouchException(string.Format("Input provider '{0}' could not be found", provider));

			IProvider activatedProvider = currentProviderToken.Activate<IProvider>(AppDomain.CurrentDomain);
			providerManager = new InputProviderManager(activatedProvider);
		}

		public void Stop()
		{
			if (providerManager != null)
			{
				providerManager.Dispose();
				providerManager = null;
			}
		}
	}
}
