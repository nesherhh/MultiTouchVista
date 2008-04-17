using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Danilins.Multitouch.Logic.LegacySupport
{
	static class Send
	{
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

		public static void Down(IntPtr hWnd, Point point)
		{
			Console.WriteLine("Sending to {0} - WM_LBUTTONDOWN", hWnd);
			SendMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, MakeLParam((int)point.X, (int)point.Y));
		}

		public static void Move(IntPtr hWnd, Point point)
		{
			Console.WriteLine("Sending to {0} - WM_MOUSEMOVE: {1}", hWnd, point);
			SendMessage(hWnd, WM_MOUSEMOVE, 0, MakeLParam((int)point.X, (int)point.Y));
		}

		public static void Up(IntPtr hWnd, Point point)
		{
			Console.WriteLine("Sending to {0} - WM_LBUTTONUP", hWnd);
			SendMessage(hWnd, WM_LBUTTONUP, MK_LBUTTON, MakeLParam((int)point.X, (int)point.Y));
		}
	}
}