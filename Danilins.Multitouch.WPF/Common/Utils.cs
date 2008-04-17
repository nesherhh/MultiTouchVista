using System;

namespace Danilins.Multitouch.Common
{
	public static class Utils
	{
		private static Random random;
		static Utils()
		{
			random = new Random();
		}
		
		public static int NextRandom(int minValue, int maxValue)
		{
			return random.Next(minValue, maxValue);
		}

		public static int NextRandom(int maxValue)
		{
			return random.Next(maxValue);
		}

		public static double NextRandom()
		{
			return random.NextDouble();
		}
	}
}
