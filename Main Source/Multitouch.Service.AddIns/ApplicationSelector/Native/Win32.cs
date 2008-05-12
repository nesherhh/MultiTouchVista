using System;
using System.Runtime.InteropServices;

namespace ApplicationSelector.Native
{
	class Win32
	{
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}
}
