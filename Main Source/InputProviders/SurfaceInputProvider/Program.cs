using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Surface;
using Microsoft.Surface.Core;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace SurfaceInputProvider
{
	static class Program
	{
		// Hold on to the game window.
		static GameWindow Window;
		public static App1 app;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Start(InputProvider inputProvider)
		{
			// Disable the WinForms unhandled exception dialog.
			// SurfaceShell will notify the user.
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

			// Apply Surface globalization settings
			GlobalizationSettings.ApplyToCurrentThread();

			using (app = new App1(inputProvider))
			{
				app.Run();
			}
		}

		/// <summary>
		/// Sets the window style for the specified HWND to None.
		/// </summary>
		/// <param name="hWnd">the handle of the window</param>
		internal static void RemoveBorder(IntPtr hWnd)
		{
			Form form = (Form)Form.FromHandle(hWnd);
			form.FormBorderStyle = FormBorderStyle.None;
			form.WindowState = FormWindowState.Minimized;

			form.TopMost = true;
			form.ShowInTaskbar = false;

			form.BackColor = form.TransparencyKey = Color.Black;

			SetWindowLong(hWnd, GWL_EXSTYLE, (IntPtr)(GetWindowLong(hWnd, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT));
			// set transparency to 50% (128)
			SetLayeredWindowAttributes(hWnd, 0, 0, LWA_ALPHA);
		}

		/// <summary>
		/// Registers event handlers and sets the initial position of the game window.
		/// </summary>
		/// <param name="window">the game window</param>
		internal static void PositionWindow(GameWindow window)
		{
			if (window == null)
				throw new ArgumentNullException("window");

			if (Window != null)
			{
				Window.ClientSizeChanged -= OnSetWindowPosition;
				Window.ScreenDeviceNameChanged -= OnSetWindowPosition;
			}

			Window = window;

			Window.ClientSizeChanged += OnSetWindowPosition;
			Window.ScreenDeviceNameChanged += OnSetWindowPosition;

			UpdateWindowPosition();
		}

		/// <summary>
		/// When the ScreenDeviceChanges or the ClientSizeChanges update the Windows Position.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void OnSetWindowPosition(object sender, EventArgs e)
		{
			UpdateWindowPosition();
		}

		/// <summary>
		/// Use the Desktop bounds to update the the position of the Window correctly.
		/// </summary>
		private static void UpdateWindowPosition()
		{
			IntPtr hWnd = Window.Handle;
			Form form = (Form)Form.FromHandle(hWnd);
			form.SetDesktopLocation(InteractiveSurface.DefaultInteractiveSurface.Left - (Window.ClientBounds.Left - form.DesktopBounds.Left),
									InteractiveSurface.DefaultInteractiveSurface.Top - (Window.ClientBounds.Top - form.DesktopBounds.Top));
			form.WindowState = FormWindowState.Minimized;
		}


		[DllImport("user32.dll", SetLastError = true)]
		private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll")]
		static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		public const int GWL_EXSTYLE = -20;
		public const int WS_EX_LAYERED = 0x80000;
		public const int WS_EX_TRANSPARENT = 0x20;
		public const int LWA_ALPHA = 0x2;
		public const int LWA_COLORKEY = 0x1;
	}
}

