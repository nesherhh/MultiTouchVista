using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Multitouch.Driver.Logic
{
	static class Utility
	{
		[DllImport("user32.dll", EntryPoint = "MapWindowPoints")]
		static extern int MapWindowPoints([In] IntPtr hWndFrom, [In] IntPtr hWndTo, ref POINT lpPoints, uint cPoints);

		public static POINT ToPOINT(this Point point)
		{
			return new POINT((int)(point.X + 0.5), (int)(point.Y + 0.5));
		}

		public static Point ToPoint(this POINT point)
		{
			return new Point(point.X, point.Y);
		}

		public static Point ScreenToClient(Point point, IntPtr clientHandle)
		{
			POINT p = point.ToPOINT();
			MapWindowPoints(IntPtr.Zero, clientHandle, ref p, 1);
			return p.ToPoint();
		}

		public static Point ClientToScreen(Point point, IntPtr clientHandle)
		{
			POINT p = point.ToPOINT();
			MapWindowPoints(clientHandle, IntPtr.Zero, ref p, 1);
			return p.ToPoint();			
		}

		public static Rect ScreenToClient(Rect rect, IntPtr clientHandle)
		{
			Point location = ScreenToClient(rect.Location, clientHandle);
			return new Rect(location, rect.Size);
		}

		/// <summary>
		/// Wrapper around the Winapi POINT type.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			/// <summary>
			/// The X Coordinate.
			/// </summary>
			public int X;

			/// <summary>
			/// The Y Coordinate.
			/// </summary>
			public int Y;

			/// <summary>
			/// Creates a new POINT.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}
}