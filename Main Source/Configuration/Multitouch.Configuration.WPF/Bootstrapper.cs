using System;
using System.Windows;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;

namespace Multitouch.Configuration.WPF
{
	class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			MainWindow mainWindow = Container.Resolve<MainWindow>();
			mainWindow.Show();
			return mainWindow;
		}

		protected override IModuleCatalog GetModuleCatalog()
		{
			return new ModuleCatalog();
		}
	}
}
