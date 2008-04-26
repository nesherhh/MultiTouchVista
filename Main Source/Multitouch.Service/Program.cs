using System;
using System.Windows.Forms;
using Multitouch.Service.Views;

namespace Multitouch.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//ServiceBase[] ServicesToRun;
			//ServicesToRun = new ServiceBase[] 
			//{ 
			//    new MultitouchService() 
			//};
			//ServiceBase.Run(ServicesToRun);

			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new Context());

			MultitouchInput input = new MultitouchInput();
			input.Start();
			Console.WriteLine("Multi-touch input service is running.");
			Console.WriteLine("Press ENTER to stop and exit.");
			Console.ReadLine();
			input.Stop();
		}
	}
}
