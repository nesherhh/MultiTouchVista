using System;
using Multitouch.Service.Logic;

namespace Multitouch.Service.Console
{
	class Program
	{
		static void Main()
		{
			MultitouchInput input = new MultitouchInput();
			input.Start();
			System.Console.WriteLine("Multi-touch input service is running.");
			System.Console.WriteLine("Press ENTER to stop and exit.");
			System.Console.ReadLine();
			input.Stop();
		}
	}
}
