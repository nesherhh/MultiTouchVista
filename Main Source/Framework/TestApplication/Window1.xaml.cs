using System;
using System.Windows;

namespace TestApplication
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void Button_NewContact(object sender, System.Windows.RoutedEventArgs e)
		{
			MessageBox.Show("new contact");
		}

		private void Button_ContactMoved(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("contact moved");
		}

		private void Button_ContactRemoved(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("contact removed");
		}
	}
}
