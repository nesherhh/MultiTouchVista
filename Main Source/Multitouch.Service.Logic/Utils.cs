using System;
using System.Runtime.InteropServices;
using System.Windows;
using ManagedWinapi.Windows;

namespace Multitouch.Service.Logic
{
	static class Utils
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

		public static IntPtr GetWindowFromPoint(Point position)
		{
			SystemWindow window = SystemWindow.FromPoint((int)(position.X + 0.5), (int)(position.Y+ 0.5));
			return window.HWnd;
		}

		public static Point ScreenToClient(Point point, IntPtr clientHandle)
		{
			POINT p = point.ToPOINT();
			MapWindowPoints(IntPtr.Zero, clientHandle, ref p, 1);
			return p.ToPoint();
		}

		public static Rect ScreenToClient(Rect rect, IntPtr clientHandle)
		{
			Point location = ScreenToClient(rect.Location, clientHandle);
			return new Rect(location, rect.Size);
		}
	}
}
