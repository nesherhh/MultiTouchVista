using System;
using DWMaxxAddIn.Native;
using ManagedWinapi.Windows;

namespace DWMaxxAddIn
{
	static class Extensions
	{
		public static System.Windows.Point ToPoint(this System.Drawing.Point point)
		{
			return new System.Windows.Point(point.X, point.Y);
		}

		public static System.Drawing.Point ToPoint(this System.Windows.Point point)
		{
			return new System.Drawing.Point((int)point.X, (int)point.Y);
		}

		public static NativeMethods.WNDCLASSEX GetClassInfo(this SystemWindow window)
		{
			NativeMethods.WNDCLASSEX wc= new NativeMethods.WNDCLASSEX();
			if (!NativeMethods.GetClassInfoEx(IntPtr.Zero, window.ClassName, wc))
				return null;
			return wc;
		}
	}
}