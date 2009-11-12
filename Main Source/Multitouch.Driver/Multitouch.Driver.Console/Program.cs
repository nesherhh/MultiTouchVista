using Multitouch.Driver.Logic;
using Multitouch.Service.Logic;

namespace Multitouch.Driver.Console
{
	using System;

	class Program
	{
		private static MultitouchInput input;
		private static MultitouchDriver driver;

		private static void Main(string[] args)
		{
			if(args.Length == 1)
			{
				string parameter = args[0];
				if(parameter.Equals("-standalone"))
				{
					input = new MultitouchInput();
					input.Start();					
				}
			}

			driver = new MultitouchDriver();
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
