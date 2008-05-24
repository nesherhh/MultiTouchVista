using System;
using System.Runtime.InteropServices;

namespace PostSingleInput.Native
{
	static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

		public const uint WM_MOUSEMOVE = 0x0200;
		public const uint WM_LBUTTONDOWN = 513; // 0x0201 
		public const uint WM_LBUTTONUP = 514; // 0x0202 

		public const int MK_LBUTTON = 0x0001;

		[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 2)]
		public struct MAKELPARAM
		{
			public uint wLow;
			public uint wHigh;

			public MAKELPARAM(uint low, uint high)
			{
				wLow = low;
				wHigh = high;
			}
		}

		public static IntPtr MakeLParam(int LoWord, int HiWord)
		{
			return (IntPtr)((HiWord << 16) + (LoWord));
		}

		public static void SendMouseDown(IntPtr hWnd, POINT point)
		{
			Console.WriteLine("Sending to {0} - WM_LBUTTONDOWN", hWnd);
			SendMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, MakeLParam(point.x, point.y));
		}

		public static void SendMouseMove(IntPtr hWnd, POINT point)
		{
			Console.WriteLine("Sending to {0} - WM_MOUSEMOVE: {1}", hWnd, point);
			SendMessage(hWnd, WM_MOUSEMOVE, 0, MakeLParam(point.x, point.y));
		}

		public static void SendMouseUp(IntPtr hWnd, POINT point)
		{
			Console.WriteLine("Sending to {0} - WM_LBUTTONUP", hWnd);
			SendMessage(hWnd, WM_LBUTTONUP, MK_LBUTTON, MakeLParam(point.x, point.y));
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);
	}
}
