using System;
using System.Windows;

namespace Multitouch.Framework.WPF.Controls.Primitives
{
	/// <summary>
	/// ScrollBar that respond to Multitouch events
	/// </summary>
	public class ScrollBar : System.Windows.Controls.Primitives.ScrollBar
	{
		static ScrollBar()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollBar), new FrameworkPropertyMetadata(typeof(ScrollBar)));
		}
	}
}