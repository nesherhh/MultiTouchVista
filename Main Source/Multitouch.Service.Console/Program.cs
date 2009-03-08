using System;
using Multitouch.Service.Logic;

namespace Multitouch.Service.Console
{
	using Console = System.Console;

	class Program
	{
		[STAThread]
		static void Main()
		{
			MultitouchInput input;
			try
			{
				input = new MultitouchInput();
			}
			catch (Exception e)
			{
				DisplayError(e);
				Console.WriteLine("Press ENTER to exit.");
				Console.ReadLine();
				return;
			}
			try
			{
				input.Start();
			}
			catch (Exception e)
			{
				DisplayError(e);
			}
			Console.WriteLine("Multi-touch input service is running.");
			Console.WriteLine("Press ENTER to stop and exit.");
			Console.ReadLine();
			Console.WriteLine("Stopping service...");
			input.Stop();
			Console.WriteLine("Service stopped.");
		}

		static void DisplayError(Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e.Message);
			Console.ResetColor();
		}
	}
}