using System;
using Multitouch.Driver.Logic;

namespace Multitouch.Driver.Console
{
	class Program
	{
		static void Main()
		{
			MultitouchDriver driver = new MultitouchDriver();
			try
			{
				driver.Start();
				System.Console.WriteLine("Multitouch driver is running.");
				System.Console.WriteLine("Press ENTER to stop and exit.");

				System.Console.ReadLine();

				System.Console.WriteLine("Stopping service...");
				driver.Stop();
				System.Console.WriteLine("Service stopped.");
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.ToString());
			}
		}
	}
}
