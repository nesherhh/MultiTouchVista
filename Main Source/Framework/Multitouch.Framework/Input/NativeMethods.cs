using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Multitouch.Framework.Input
{
	static class NativeMethods
	{
        [DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] uint cPoints);

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

		public static POINT ScreenToClient(POINT point, IntPtr clientHandle)
		{
			MapWindowPoints(IntPtr.Zero, clientHandle, ref point, 1);
			return point;
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

		internal delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

		[StructLayout(LayoutKind.Sequential)]
		internal struct MSLLHOOKSTRUCT
		{
			public POINT pt;
			public uint mouseData;
			public uint flags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		internal const int WH_MOUSE_LL = 14;

		internal enum MouseMessages
		{
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_MOUSEMOVE = 0x0200,
			WM_MOUSEWHEEL = 0x020A,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("user32.dll")]
		internal static extern IntPtr WindowFromPoint(POINT Point);
	}
}
