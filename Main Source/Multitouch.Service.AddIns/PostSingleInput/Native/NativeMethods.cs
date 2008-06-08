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
			//Console.WriteLine("Sending to {0} - WM_LBUTTONDOWN", hWnd);
			SendMessage(hWnd, WM_LBUTTONDOWN, MK_LBUTTON, MakeLParam(point.x, point.y));
		}

		public static void SendMouseMove(IntPtr hWnd, POINT point)
		{
			//Console.WriteLine("Sending to {0} - WM_MOUSEMOVE: {1}", hWnd, point);
			SendMessage(hWnd, WM_MOUSEMOVE, 0, MakeLParam(point.x, point.y));
		}

		public static void SendMouseUp(IntPtr hWnd, POINT point)
		{
			//Console.WriteLine("Sending to {0} - WM_LBUTTONUP", hWnd);
			SendMessage(hWnd, WM_LBUTTONUP, MK_LBUTTON, MakeLParam(point.x, point.y));
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT
		{
			[FieldOffset(0)]
			public int type;
			[FieldOffset(4)]
			public MOUSEINPUT mi;
			[FieldOffset(4)]
			public KEYBDINPUT ki;
			[FieldOffset(4)]
			public HARDWAREINPUT hi;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HARDWAREINPUT
		{
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}

		public const int INPUT_MOUSE = 0;
		public const int INPUT_KEYBOARD = 1;
		public const int INPUT_HARDWARE = 2;
		public const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		public const uint KEYEVENTF_KEYUP = 0x0002;
		public const uint KEYEVENTF_UNICODE = 0x0004;
		public const uint KEYEVENTF_SCANCODE = 0x0008;
		public const uint XBUTTON1 = 0x0001;
		public const uint XBUTTON2 = 0x0002;
		public const uint MOUSEEVENTF_MOVE = 0x0001;
		public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
		public const uint MOUSEEVENTF_LEFTUP = 0x0004;
		public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
		public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
		public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
		public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
		public const uint MOUSEEVENTF_XDOWN = 0x0080;
		public const uint MOUSEEVENTF_XUP = 0x0100;
		public const uint MOUSEEVENTF_WHEEL = 0x0800;
		public const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
		public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
	}
}
