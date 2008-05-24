using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Multitouch.Framework.WPF.Native
{
	static class Win32
	{
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
				this.X = x;
				this.Y = y;
			}

			/// <summary>
			/// Implicit cast.
			/// </summary>
			/// <returns></returns>
			public static implicit operator Point(POINT p)
			{
				return new Point(p.X, p.Y);
			}

			/// <summary>
			/// Implicit cast.
			/// </summary>
			/// <returns></returns>
			public static implicit operator POINT(Point p)
			{
				return new POINT((int)p.X, (int)p.Y);
			}
		}

		[DllImport("user32.dll")]
		private static extern IntPtr WindowFromPoint(POINT Point);

		public static DependencyObject GetWindowAt(Point point)
		{
			DependencyObject reference = null;

			POINT p = point;
			IntPtr hwnd = WindowFromPoint(p);
			HwndSource hwndSource = HwndSource.FromHwnd(hwnd);
			Visual rootVisual = hwndSource.RootVisual;
			HitTestResult result = VisualTreeHelper.HitTest(rootVisual, point);
			if (result != null)
				reference = result.VisualHit;
			return reference;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);

		[DllImport("user32.dll")]
		public static extern IntPtr GetMessageExtraInfo();
	}
}
