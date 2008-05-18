using System;
using System.ServiceProcess;

namespace Multitouch.Service
{
	public partial class MultitouchService : ServiceBase
	{
		IDisposable processManager;

		public MultitouchService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			processManager = ProcessManager.Start();
		}

		protected override void OnStop()
		{
			if(processManager != null)
				processManager.Dispose();
		}
	}
}
