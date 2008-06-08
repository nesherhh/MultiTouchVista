using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PostSingleInput.Native;

namespace PostSingleInput
{
	static class MouseEmulation
	{
		public enum MouseAction
		{
			LeftDown,
			LeftUp,
			Move,
			RightDown,
			RightUp
		}

		public static void Action(double x, double y, MouseAction buttonState)
		{
			Point point = new Point((int)x, (int)y);
			Cursor.Position = point;
			
			NativeMethods.INPUT[] input = new NativeMethods.INPUT[1];

			NativeMethods.MOUSEINPUT mi = new NativeMethods.MOUSEINPUT();
			mi.dx = 0;
			mi.dy = 0;
			if (buttonState == MouseAction.LeftDown)
				mi.dwFlags = NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE;
			else if (buttonState == MouseAction.LeftUp)
				mi.dwFlags = NativeMethods.MOUSEEVENTF_LEFTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE;
			else if (buttonState == MouseAction.RightDown)
				mi.dwFlags = NativeMethods.MOUSEEVENTF_RIGHTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE;
			else if (buttonState == MouseAction.RightUp)
				mi.dwFlags = NativeMethods.MOUSEEVENTF_RIGHTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE;
			input[0] = new NativeMethods.INPUT();
			input[0].mi = mi;

			NativeMethods.SendInput((uint)input.Length, input, Marshal.SizeOf(input[0]));
		}
	}
}