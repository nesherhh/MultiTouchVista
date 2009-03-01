using System;
using System.Windows;
using MultipleMice.Properties;

namespace MultipleMice
{
	/// <summary>
	/// Interaction logic for ConfigurationWindow.xaml
	/// </summary>
	public partial class ConfigurationWindow
	{
		public static readonly DependencyProperty BlockMouseProperty =
			DependencyProperty.Register("BlockMouse", typeof(bool), typeof(ConfigurationWindow), new UIPropertyMetadata(false));

		public bool BlockMouse
		{
			get { return (bool)GetValue(BlockMouseProperty); }
			set { SetValue(BlockMouseProperty, value); }
		}

		public ConfigurationWindow()
		{
			DataContext = this;
			InitializeComponent();

			BlockMouse = Settings.Default.BlockMouse;
		}

		private void ok_Click(object sender, RoutedEventArgs e)
		{
			Settings.Default.BlockMouse = BlockMouse;
			Settings.Default.Save();
			Close();
		}

		private void cancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
			Settings.Default.Reload();
		}
	}
}