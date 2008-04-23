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

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Context());
		}
	}
}
