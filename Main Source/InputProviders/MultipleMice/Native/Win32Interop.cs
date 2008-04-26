using System;
using System.Runtime.InteropServices;

namespace MultipleMice.Native
{
	internal sealed class Win32Interop
	{
		// Fields
		internal const byte AC_SRC_ALPHA = 1;
		internal const byte AC_SRC_OVER = 0;
		internal const int APPCOMMAND_BROWSER_BACKWARD = 1;
		internal const int APPCOMMAND_BROWSER_FORWARD = 2;
		internal const int FAPPCOMMAND_MOUSE = 32768;
		internal const int GWL_EXSTYLE = -20;
		internal const uint INPUT_HARDWARE = 2;
		internal const uint INPUT_KEYBOARD = 1;
		internal const uint INPUT_MOUSE = 0;
		internal const int KeyboardDelayMax = 3;
		internal const int KeyboardDelayMin = 0;
		internal const int KeyboardSpeedMax = 31;
		internal const int KeyboardSpeedMin = 0;
		internal const int KEYEVENTF_EXTENDEDKEY = 1;
		internal const int KEYEVENTF_KEYUP = 2;
		internal const int KEYEVENTF_SCANCODE = 8;
		internal const int KEYEVENTF_UNICODE = 4;
		internal const int MA_ACTIVATE = 1;
		internal const int MA_ACTIVATEANDEAT = 2;
		internal const int MA_NOACTIVATE = 3;
		internal const int MA_NOACTIVATEANDEAT = 4;
		internal const ushort MK_LBUTTON = 1;
		internal const ushort MK_RBUTTON = 2;
		internal const uint MOUSEEVENTF_ABSOLUTE = 32768;
		internal const uint MOUSEEVENTF_RIGHTDOWN = 8;
		internal const uint MOUSEEVENTF_RIGHTUP = 16;
		internal const uint MOUSEEVENTF_VIRTUALDESK = 16384;
		internal const uint MOUSEEVENTF_WHEEL = 2048;
		internal const int SB_LINEDOWN = 1;
		internal const int SB_LINEUP = 0;
		internal const int SB_PAGEDOWN = 3;
		internal const int SB_PAGEUP = 2;
		internal const int SM_CXVIRTUALSCREEN = 78;
		internal const int SM_CYVIRTUALSCREEN = 79;
		internal const int SM_XVIRTUALSCREEN = 76;
		internal const int SM_YVIRTUALSCREEN = 77;
		internal const uint SMTO_ABORTIFHUNG = 2;
		internal const uint SMTO_BLOCK = 1;
		internal const uint SMTO_NORMAL = 0;
		internal const uint SMTO_NOTIMEOUTIFNOTHUNG = 8;
		internal const uint SPI_GETKEYBOARDDELAY = 22;
		internal const uint SPI_GETKEYBOARDSPEED = 10;
		internal const uint SPI_GETWHEELSCROLLLINES = 104;
		internal const uint SPI_SETWHEELSCROLLLINES = 105;
		internal const uint SPIF_SENDWININICHANGE = 2;
		internal const uint SPIF_UPDATEINIFILE = 1;
		internal const int ULW_ALPHA = 2;
		internal const int ULW_COLORKEY = 1;
		internal const int ULW_OPAQUE = 4;
		internal const ushort VK_CONTROL = 17;
		internal const ushort VK_DELETE = 46;
		internal const ushort VK_END = 35;
		internal const ushort VK_ESCAPE = 27;
		internal const ushort VK_F4 = 115;
		internal const ushort VK_HOME = 36;
		internal const ushort VK_LWIN = 91;
		internal const ushort VK_MENU = 18;
		internal const ushort VK_RETURN = 13;
		internal const ushort VK_RWIN = 92;
		internal const ushort VK_SHIFT = 16;
		internal const ushort VK_TAB = 9;
		internal const uint WHEEL_DELTA = 120;
		internal const uint WHEEL_PAGESCROLL = uint.MaxValue;
		internal const int WM_APPCOMMAND = 793;
		internal const int WM_HSCROLL = 276;
		internal const int WM_MOUSEACTIVATE = 33;
		internal const int WM_MOUSEWHEEL = 522;
		internal const int WM_RBUTTONDOWN = 516;
		internal const int WM_RBUTTONUP = 517;
		internal const int WM_VSCROLL = 277;
		internal const int WS_EX_LAYERED = 524288;
		internal const int WS_EX_NOACTIVATE = 134217728;
		internal const int WS_EX_TOOLWINDOW = 128;
		internal const int WS_EX_TRANSPARENT = 32;

		// Methods
		[DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);
		[DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL DeleteDC(IntPtr hdc);
		[DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL DeleteObject(IntPtr hObject);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern IntPtr FindWindowA([MarshalAs(UnmanagedType.LPStr)] string lpClassName, [MarshalAs(UnmanagedType.LPStr)] string lpWindowName);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL PostMessageW(IntPtr hWnd, uint msg, int wParam, int lParam);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern uint RegisterWindowMessageW([MarshalAs(UnmanagedType.LPWStr)] string lpString);
		[DllImport("user32.dll", ExactSpelling = true)]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern BOOL ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
		[DllImport("gdi32.dll", ExactSpelling = true)]
		internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int SendInput(int nInputs, INPUT[] pInputs, int cbSize);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL SendMessageTimeoutW(IntPtr hWnd, uint msg, int wParam, int lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL SendMessageW(IntPtr hWnd, uint msg, int wParam, int lParam);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL SystemParametersInfoW(uint uiAction, uint uiParam, out IntPtr pvParam, uint fWinIni);
		[DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
		internal static extern BOOL UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
		[DllImport("user32.dll", ExactSpelling = true)]
		internal static extern IntPtr WindowFromPoint(POINT Point);

		// Nested Types
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct ARGB
		{
			public byte Blue;
			public byte Green;
			public byte Red;
			public byte Alpha;
			public ARGB(byte Alpha, byte Red, byte Green, byte Blue)
			{
				this.Alpha = Alpha;
				this.Red = Red;
				this.Green = Green;
				this.Blue = Blue;
			}
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BLENDFUNCTION
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		public enum BOOL
		{
			FALSE,
			TRUE
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct HARDWAREINPUT
		{
			internal uint uMsg;
			internal ushort wParamL;
			internal ushort wParamH;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct INPUT
		{
			internal uint type;
			internal Win32Interop.INPUTUNION data;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct INPUTUNION
		{
			// Fields
			[FieldOffset(0)]
			internal Win32Interop.HARDWAREINPUT hi;
			[FieldOffset(0)]
			internal Win32Interop.KBDINPUT ki;
			[FieldOffset(0)]
			internal Win32Interop.MOUSEINPUT mi;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct KBDINPUT
		{
			internal ushort vKey;
			internal ushort scanCode;
			internal uint flags;
			internal uint time;
			internal UIntPtr extraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MOUSEINPUT
		{
			internal int dx;
			internal int dy;
			internal uint mouseData;
			internal uint dwFlags;
			internal uint time;
			internal UIntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SIZE
		{
			public int cx;
			public int cy;
			public SIZE(int cx, int cy)
			{
				this.cx = cx;
				this.cy = cy;
			}
		}
	}
}