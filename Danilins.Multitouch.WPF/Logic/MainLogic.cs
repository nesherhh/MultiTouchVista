using System;
using System.ComponentModel.Design;
using System.Windows.Threading;

namespace Danilins.Multitouch.Logic
{
	public class MainLogic : Common.Logics.Logic
	{
		static volatile MainLogic instance;
		private Dispatcher uiDispatcher;

		private MainLogic()
		{
			IServiceContainer container = this;
			container.AddService(typeof(MainLogic), this);
			container.AddService(typeof(InputProviderLogic), delegate { return new InputProviderLogic(this); });
			container.AddService(typeof(ServiceLogic), delegate { return new ServiceLogic(this); });
			container.AddService(typeof(ConfigurationLogic), delegate { return new ConfigurationLogic(this); });
			container.AddService(typeof(LegacySupportLogic), delegate { return new LegacySupportLogic(this); });
		}

		public static MainLogic Instance
		{
			get
			{
				if (instance == null)
					instance = new MainLogic();
				return instance;
			}
		}

		public InputProviderLogic InputProviderLogic
		{
			get { return GetLogic<InputProviderLogic>(); }
		}

		public ServiceLogic ServiceLogic
		{
			get { return GetLogic<ServiceLogic>(); }
		}

		public ConfigurationLogic ConfigurationLogic
		{
			get { return GetLogic<ConfigurationLogic>(); }
		}

		public Dispatcher UIDispatcher
		{
			get { return uiDispatcher; }
			set { uiDispatcher = value; }
		}
	}
}