using System;
using System.Windows;

namespace Multitouch.Framework.WPF.Tests
{
	public class RoutedEventArgsInfo
	{
		public RoutedEventArgsInfo(RoutedEventArgs e)
		{
			RoutedEvent = e.RoutedEvent;
			Source = e.Source;
			OriginalSource = e.OriginalSource;
			Handled = e.Handled;
			EventArgs = e;
		}

		public bool Handled { get; private set; }

		public object OriginalSource { get; private set; }

		public RoutedEvent RoutedEvent { get; private set; }

		public object Source { get; private set; }

		public RoutedEventArgs EventArgs { get; private set; }
	}
}
