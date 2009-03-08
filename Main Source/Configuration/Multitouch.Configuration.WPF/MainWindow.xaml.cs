using System;
using Microsoft.Practices.Unity;
using Multitouch.Configuration.WPF.Models;

namespace Multitouch.Configuration.WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		[Dependency]
		public ConfigurationModel Model
		{
			get { return (ConfigurationModel)DataContext; }
			set { DataContext = value; }
		}

		public MainWindow()
		{
			InitializeComponent();
		}
	}
}
