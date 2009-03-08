using System;
using Multitouch.Driver.Logic;

namespace Multitouch.Driver.Console
{
	using Console = System.Console;

	class Program
	{
		static void Main()
		{
			MultitouchDriver driver = new MultitouchDriver();
			try
			{
				driver.Start();
				Console.WriteLine("Multitouch driver is running.");
				Console.WriteLine("Press ENTER to stop and exit.");

				Console.ReadLine();

				Console.WriteLine("Stopping service...");
				driver.Stop();
				Console.WriteLine("Service stopped.");
			}
			catch (Exception e)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(e.Message);
				Console.ResetColor();
			}
		}
	}
}
