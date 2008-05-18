using System;
using System.Diagnostics;
using System.ServiceProcess;
using Multitouch.Service.Logic;

namespace Multitouch.Service.Logic
{
	public partial class MultitouchService : ServiceBase
	{
		MultitouchInput multitouchInput;

		public MultitouchService()
		{
			InitializeComponent();
			Debugger.Launch();
		}

		protected override void OnStart(string[] args)
		{
			multitouchInput = new MultitouchInput();
			multitouchInput.Start();
		}

		protected override void OnStop()
		{
			if (multitouchInput != null)
			{
				multitouchInput.Stop();
				multitouchInput = null;
			}
		}
	}
}