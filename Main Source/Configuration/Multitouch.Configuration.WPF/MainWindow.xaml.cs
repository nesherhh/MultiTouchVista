using System;
using System.Windows;
using Multitouch.Configuration.WPF.Models;

namespace Multitouch.Configuration.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ConfigurationModel model = (ConfigurationModel)DataContext;
			try
			{
				model.Initialize();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
