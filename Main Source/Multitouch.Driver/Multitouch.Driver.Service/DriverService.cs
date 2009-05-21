using System;
using System.ServiceProcess;

namespace Multitouch.Driver.Service
{
	partial class DriverService : ServiceBase
	{
		private IDisposable processManager;

		public DriverService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			processManager = ProcessManager.Start();
		}

		protected override void OnStop()
		{
			if (processManager != null)
				processManager.Dispose();
		}
	}
}