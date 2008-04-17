using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MultiTouchVista
{
	public static class API
	{
		#region  General Structures 

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINTAPI
		{
			public UInt32 X;
			public UInt32 Y;
		}

		#endregion

		#region Shell Hooking

		public const int WS_SYSMENU = 0x80000;

		public const short GW_OWNER = 4;
		public const short GWL_STYLE = (-16);
		public const short GWL_EXSTYLE = (-20);

		public const short GWL_WNDPROC = (-4);
		public const short RSH_DEREGISTER = 0;

		public delegate int WndProcDelegate(int hwnd, int wMsg,int wParam,int lParam);

		[DllImport("user32", EntryPoint = "CallWindowProcA")]
		public static extern int CallWindowProc(int lpPrevWndFunc, int hwnd, int MSG, int wParam, int lParam);

		[DllImport("Shell32", EntryPoint = "#181")]
		public static extern int RegisterShellHook(int hwnd, int nAction);

		[DllImport("user32", EntryPoint = "RegisterWindowMessageA")]
		public static extern int RegisterWindowMessage(String lpString);

		[DllImport("user32", EntryPoint = "GetWindowLongA")]
		public static extern int GetWindowLong(int hwnd, int nIndex);

		[DllImport("user32", EntryPoint = "SetWindowLongA")]
		public static extern int SetWindowLong(int hwnd, int nIndex, int dwNewLong);
		[DllImport("user32", EntryPoint = "SetWindowLongA")]
		public static extern int SetWindowLong2(int hwnd, int nIndex, WndProcDelegate dwNewLong);

		[DllImport("user32", EntryPoint = "IsWindow")]
		public static extern int IsWindow(int hwnd);

		[DllImport("user32", EntryPoint = "IsWindowVisible")]
		public static extern int IsWindowVisible(int hwnd);

		[DllImport("user32", EntryPoint = "GetWindow")]
		public static extern int GetWindow(int hwnd, int wCmd);

		public delegate int EnumWindowsDelegate(int lpEnumFunc, int lParam);
		[DllImport("user32", EntryPoint = "EnumWindows")]
		public static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, int lParam);

		[DllImport("user32", EntryPoint = "GetWindowRect")]
		public static extern int GetWindowRect(IntPtr hwnd, RECT lpRect);
    
		[DllImport("user32", EntryPoint = "SetWindowPos")]
		public static extern int SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

		[DllImport("user32", EntryPoint = "GetWindowTextA")]
		public static extern int GetWindowText(int hWnd, StringBuilder text, int count);


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

		#endregion

	}
}
