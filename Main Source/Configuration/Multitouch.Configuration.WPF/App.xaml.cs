using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Composite;

namespace Multitouch.Configuration.WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		public App()
		{
			DispatcherUnhandledException += App_DispatcherUnhandledException;
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			new Bootstrapper().Run();
		}

		void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Exception exception = e.Exception.GetRootException();
			MessageBox.Show(exception.Message);
			e.Handled = true;
			
			Shutdown();
		}
	}
}
