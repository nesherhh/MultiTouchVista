using System;
using Multitouch.Service.Logic;

namespace Multitouch.Service.Console
{
	class Program
	{
		[STAThread]
		static void Main()
		{
			MultitouchInput input = new MultitouchInput();
			input.Start();
			System.Console.WriteLine("Multi-touch input service is running.");
			System.Console.WriteLine("Press ENTER to stop and exit.");

			System.Console.ReadLine();
			System.Console.WriteLine("Stopping service...");
			input.Stop();
			System.Console.WriteLine("Service stopped.");
		}
	}
}
