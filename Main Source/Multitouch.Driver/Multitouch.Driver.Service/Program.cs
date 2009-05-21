using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Multitouch.Driver.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			if (args.Length == 0)
				ServiceBase.Run(new DriverService());
			else
			{
				if (args[0].Equals(ProcessManager.EMBEDDING, StringComparison.InvariantCultureIgnoreCase))
					Application.Run(new Context());
			}
		}
	}
}