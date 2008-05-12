using System;
using System.ComponentModel;
using System.Windows;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls
{
	public class Window : System.Windows.Window
	{
		MultitouchInputProvider inputProvider;

		static Window()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			inputProvider = new MultitouchInputProvider(PresentationSource.FromVisual(this));
			base.OnSourceInitialized(e);
		}

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