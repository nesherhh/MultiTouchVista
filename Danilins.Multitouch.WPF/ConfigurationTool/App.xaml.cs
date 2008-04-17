using System;
using System.Windows;
using System.Windows.Data;
using Danilins.Multitouch.ConfigurationTool.Models;

namespace Danilins.Multitouch.ConfigurationTool
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public MainModel MainModel
		{
			get { return (MainModel) ((ObjectDataProvider)FindResource("MainModelDS")).Data; }
		}
	}
}