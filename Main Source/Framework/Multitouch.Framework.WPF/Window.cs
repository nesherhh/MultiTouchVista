using System;
using System.Windows;

namespace Multitouch.Framework.WPF
{
	public class Window : System.Windows.Window
	{
		static Window()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
		}
	}
}
