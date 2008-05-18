using System;
using Multitouch.Service.Logic;

namespace Multitouch.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//ServiceBase.Run(new MultitouchService());

			MultitouchInput input = new MultitouchInput();
			input.Start();
			Console.WriteLine("Multi-touch input service is running.");
			Console.WriteLine("Press ENTER to stop and exit.");
			Console.ReadLine();
			input.Stop();

		}
	}
}