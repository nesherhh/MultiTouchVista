using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace Multitouch.Driver.Service
{
	[RunInstaller(true)]
	public partial class DriverInstaller : Installer
	{
		public DriverInstaller()
		{
			InitializeComponent();
		}
	}
}
