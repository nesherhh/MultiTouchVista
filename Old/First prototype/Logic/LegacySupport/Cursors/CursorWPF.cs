using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	public class CursorWPF : ICursor
	{
		public const int DefaultSize = 60;

		protected Image image;
		private bool isClosing;

		private HwndSource testHwnd;
		private const int WM_CLOSE = 0x10;
		private const int WS_EX_TOPMOST = 0x00000008;
		private const int WS_EX_TOOLWINDOW = 0x00000080;
		private const int WS_EX_NOACTIVATE = 0x08000000;
		private const int WS_EX_TRANSPARENT = 0x00000020;

		public CursorWPF(Point location, int width, int height)
		{
			image = new Image();

			HwndSourceParameters parameters = new HwndSourceParameters("Cursor", width, height);
			parameters.PositionX = (int)location.X;
			parameters.PositionY = (int)location.Y;
			parameters.UsesPerPixelOpacity = true;
			parameters.ExtendedWindowStyle = WS_EX_TOPMOST | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE | WS_EX_TRANSPARENT;
			testHwnd = new HwndSource(parameters);
			testHwnd.AddHook(Hook);

			testHwnd.RootVisual = image;
		}

		public CursorWPF(Point location)
			: this(location, DefaultSize, DefaultSize)
		{ }

		private IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == WM_CLOSE)
			{
				OnClosing();
				handled = true;
			}
			return IntPtr.Zero;
		}

		private void OnClosing()
		{
			isClosing = true;
		}

		protected bool IsClosing
		{
			get { return isClosing; }
		}

		public void Close()
		{
			if (!testHwnd.IsDisposed)
			{
				testHwnd.RemoveHook(Hook);
				testHwnd.Dispose();
			}
		}
	}
}