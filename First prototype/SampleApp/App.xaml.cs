using System;
using System.Windows;
using Danilins.Multitouch.Framework;

namespace SampleApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private MultitouchScreen screen;

		protected override void OnStartup(StartupEventArgs e)
		{
			screen = MultitouchScreen.Instace;
			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			screen.Dispose();
			base.OnExit(e);
		}
	}
}
