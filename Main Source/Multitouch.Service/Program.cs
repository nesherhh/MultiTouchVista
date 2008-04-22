using System;

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


			MultitouchInput input = new MultitouchInput();
			input.Start();
			Console.WriteLine("Multitouch service is running.");
			Console.WriteLine("Press ENTER to stop and exit.");
			Console.ReadLine();
			input.Stop();
		}
	}
}
