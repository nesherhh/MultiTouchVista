using System;
using System.Windows;
using System.Windows.Input;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.ConfigurationTool.Models;

namespace Danilins.Multitouch.ConfigurationTool.Commands
{
	class ShowConfigurationCommand : CommandModel
	{
		private MainModel mainModel;

		public ShowConfigurationCommand(MainModel model)
			: base("ShowConfigurationCommand", "_Configure", typeof(ShowConfigurationCommand))
		{
			mainModel = model;
		}

		public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (mainModel.SelectedProvider != null && mainModel.SelectedProvider.HasConfiguration);
		}

		public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			IConfigurableInputProvider provider = mainModel.Logic.InputProviderLogic.CurrentProvider as IConfigurableInputProvider;
			if (provider != null)
			{
				Window configWindow = new Window();
				configWindow.SizeToContent = SizeToContent.WidthAndHeight;
				configWindow.Content = provider.GetConfigurationUI();
				configWindow.Owner = App.Current.MainWindow;
				configWindow.ShowDialog();
			}
		}
	}
}