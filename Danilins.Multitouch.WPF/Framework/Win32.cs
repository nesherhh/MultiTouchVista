using System;
using System.Runtime.InteropServices;

namespace Danilins.Multitouch.Framework
{
	static class Win32
	{
		[DllImport("User32.dll")]
		private static extern int ShowCursor([MarshalAs(UnmanagedType.Bool)] bool bShow);

		public static void ShowSystemCursor()
		{
			ShowCursor(true);
		}

		public static void HideSystemCursor()
		{
			ShowCursor(false);
		}
	}
}
