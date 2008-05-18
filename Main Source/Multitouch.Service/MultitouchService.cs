using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Multitouch.Service
{
	public partial class MultitouchService : ServiceBase
	{
		public MultitouchService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
		}
	}
}
