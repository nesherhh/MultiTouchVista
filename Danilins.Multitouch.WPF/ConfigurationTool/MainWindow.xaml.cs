using System;
using System.ComponentModel;
using System.Windows;

namespace Danilins.Multitouch.ConfigurationTool
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			App.Current.Shutdown();
		}

		private void notifyIcon_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Normal;
			Activate();
		}
	}
}