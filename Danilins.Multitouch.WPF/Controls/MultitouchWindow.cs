using System;
using System.ComponentModel;
using System.Windows;
using Danilins.Multitouch.Framework;

namespace Danilins.Multitouch.Controls
{
	public class MultitouchWindow : Window
	{
		public static readonly RoutedEvent PreviewContactDownEvent =
			MultitouchScreen.PreviewContactDownEvent.AddOwner(typeof(MultitouchWindow));

		public static readonly RoutedEvent ContactDownEvent =
			MultitouchScreen.ContactDownEvent.AddOwner(typeof(MultitouchWindow));

		public static readonly RoutedEvent PreviewContactUpEvent =
			MultitouchScreen.PreviewContactUpEvent.AddOwner(typeof(MultitouchWindow));

		public static readonly RoutedEvent ContactUpEvent =
			MultitouchScreen.ContactUpEvent.AddOwner(typeof(MultitouchWindow));

		public static readonly RoutedEvent PreviewContactMoveEvent =
			MultitouchScreen.PreviewContactMoveEvent.AddOwner(typeof(MultitouchWindow));

		public static readonly RoutedEvent ContactMoveEvent =
			MultitouchScreen.ContactMoveEvent.AddOwner(typeof(MultitouchWindow));

		static MultitouchWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultitouchWindow), new FrameworkPropertyMetadata(typeof(MultitouchWindow)));
		}

		protected override void OnActivated(EventArgs e)
		{
			MultitouchScreen.Instace.DisableLegacySupport();
			base.OnActivated(e);
		}

		protected override void OnDeactivated(EventArgs e)
		{
			MultitouchScreen.Instace.EnableLegacySupport();
			base.OnDeactivated(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			MultitouchScreen.Instace.EnableLegacySupport();
			base.OnClosing(e);
		}
	}
}