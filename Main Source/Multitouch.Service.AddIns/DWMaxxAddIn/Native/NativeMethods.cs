using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using AdvanceMath;
using ManagedWinapi.Windows;

namespace DWMaxxAddIn.Native
{
	class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("user32.dll")]
		public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

		[DllImport("user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, [MarshalAs(UnmanagedType.U4)] int cPoints);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetWindowLong(IntPtr hWnd, GetWindowLongIndex nIndex);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("Shell32", EntryPoint = "#181")]
		public static extern int RegisterShellHook(IntPtr hWnd, int nAction);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern uint RegisterWindowMessage(string lpString);

		[DllImport("user32", EntryPoint = "SetWindowLongA")]
		public static extern int SetWindowLong2(IntPtr hwnd, int nIndex, WndProcDelegate dwNewLong);

		[DllImport("user32.dll")]
		public static extern IntPtr CallWindowProc(int lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

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

			public Vector2D ToVector2D()
			{
				return new Vector2D(X, Y);
			}
		}

		/// <summary>
		/// Wrapper around the Winapi RECT type.
		/// </summary>
		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			/// <summary>
			/// LEFT
			/// </summary>
			public int Left;

			/// <summary>
			/// TOP
			/// </summary>
			public int Top;

			/// <summary>
			/// RIGHT
			/// </summary>
			public int Right;

			/// <summary>
			/// BOTTOM
			/// </summary>
			public int Bottom;

			/// <summary>
			/// Creates a new RECT.
			/// </summary>
			public RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

			/// <summary>
			/// HEIGHT
			/// </summary>
			public int Height { get { return Bottom - Top; } }

			/// <summary>
			/// WIDTH
			/// </summary>
			public int Width { get { return Right - Left; } }

		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public POINT ptMinPosition;
			public POINT ptMaxPosition;
			public RECT rcNormalPosition;
		}

		public enum ShellHookTypes
		{
			RSH_REGISTER = 1,
			RSH_REGISTER_PROGMAN = 2,
			RSH_REGISTER_TASKMAN = 3
		}

		public enum ShellEvents
		{
			HSHELL_WINDOWCREATED = 1,
			HSHELL_WINDOWDESTROYED = 2,
			HSHELL_ACTIVATESHELLWINDOW = 3,
			HSHELL_WINDOWACTIVATED = 4,
			HSHELL_GETMINRECT = 5,
			HSHELL_REDRAW = 6,
			HSHELL_TASKMAN = 7,
			HSHELL_LANGUAGE = 8,
			HSHELL_ACCESSIBILITYSTATE = 11
		}

		[Flags]
		public enum SetWindowPosFlags : uint
		{
			SWP_NOSIZE = 0x0001,
			SWP_NOMOVE = 0x0002,
			SWP_NOZORDER = 0x0004,
			SWP_NOREDRAW = 0x0008,
			SWP_NOACTIVATE = 0x0010,
			SWP_FRAMECHANGED = 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
			SWP_SHOWWINDOW = 0x0040,
			SWP_HIDEWINDOW = 0x0080,
			SWP_NOCOPYBITS = 0x0100,
			SWP_NOOWNERZORDER = 0x0200,  /* Don't do owner Z ordering */
			SWP_NOSENDCHANGING = 0x0400  /* Don't send WM_WINDOWPOSCHANGING */
		}

		public enum GetWindowCmd : uint
		{
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4,
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6
		}

		public enum GetWindowLongIndex
		{
			GWL_WNDPROC = -4,
			GWL_ID = -12,
			GWL_STYLE = -16,
			GWL_EXSTYLE = -20
		}

		public const int WS_SYSMENU = 0x80000;
		public const short RSH_DEREGISTER = 0;
		public const short GWL_WNDPROC = -4;

		public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam);

		public static Point ScreenToClient(SystemWindow window, Point point)
		{
			POINT p = new POINT(point.X, point.Y);
			MapWindowPoints(IntPtr.Zero, window.HWnd, ref p, 1);
			return new Point(p.X, p.Y);
		}

		public static Point ClientToScreen(SystemWindow window, Point point)
		{
			POINT p = new POINT(point.X, point.Y);
			MapWindowPoints(window.HWnd, IntPtr.Zero, ref p, 1);
			return new Point(p.X, p.Y);
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class WNDCLASSEX
		{
			public int cbSize;
			public int style;
			public WndProc lpfnWndProc;
			public int cbClsExtra;
			public int cbWndExtra;
			public IntPtr hInstance = IntPtr.Zero;
			public IntPtr hIcon = IntPtr.Zero;
			public IntPtr hCursor = IntPtr.Zero;
			public IntPtr hbrBackground = IntPtr.Zero;
			public string lpszMenuName;
			public string lpszClassName;
			public IntPtr hIconSm = IntPtr.Zero;
		}

		public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetClassInfoEx(IntPtr hInst, string lpszClass, [In, Out] WNDCLASSEX wc);
	}
}