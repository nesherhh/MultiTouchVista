using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Multitouch.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			if (args.Length == 0)
				ServiceBase.Run(new MultitouchService());
			else
			{
				if (args[0].Equals(ProcessManager.EMBEDDING, StringComparison.InvariantCultureIgnoreCase))
					Application.Run(new InvisibleForm());
			}
		}
	}
}