using System;
using System.Windows;

namespace MultipleMice
{
	static class Extensions
	{
		public static Point ToPoint(this System.Drawing.Point point)
		{
			return	new Point(point.X, point.Y);
		}

		public static System.Drawing.Point ToPoint(this Point point)
		{
			return new System.Drawing.Point((int)point.X, (int)point.Y);
		}
	}
}
