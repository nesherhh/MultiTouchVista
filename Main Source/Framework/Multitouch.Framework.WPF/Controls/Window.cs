using System;
using System.ComponentModel;
using System.Windows;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls
{
	/// <summary>
	/// Extends WPF <see cref="System.Windows.Window"/> to support Multitouch events. WindowStyle is None, WindowState is Maximized.
	/// </summary>
	public class Window : System.Windows.Window
	{
		MultitouchInputProvider inputProvider;

		static Window()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Window.SourceInitialized"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnSourceInitialized(EventArgs e)
		{
			inputProvider = new MultitouchInputProvider(PresentationSource.FromVisual(this));
			base.OnSourceInitialized(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (inputProvider != null)
			{
				inputProvider.Dispose();
				inputProvider = null;
			}
			base.OnClosing(e);
		}
	}
}