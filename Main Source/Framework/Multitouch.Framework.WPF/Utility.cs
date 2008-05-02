using System;

namespace Multitouch.Framework.WPF
{
	static class Utility
	{
		static Random random = new Random(Environment.TickCount);

		public static double NextRandom()
		{
			return random.NextDouble();
		}
	}
}
