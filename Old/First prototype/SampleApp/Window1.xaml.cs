using System;
using System.IO;
using System.Linq;
using Danilins.Multitouch.Controls;

namespace SampleApp
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : MultitouchWindow
	{
		public Window1()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			string[] files = Directory.GetFiles(@"D:\Users\Daniel\Pictures", "*.jpg");

			itemsControl.ItemsSource = files.Take(5);

			base.OnInitialized(e);
		}
	}
}
