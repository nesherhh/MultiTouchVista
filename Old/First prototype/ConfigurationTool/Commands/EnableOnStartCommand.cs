using System;
using System.Windows.Controls;
using System.Windows.Input;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.ConfigurationTool.Models;

namespace Danilins.Multitouch.ConfigurationTool.Commands
{
	class EnableOnStartCommand : CommandModel
	{
		private readonly MainModel model;

		public EnableOnStartCommand(MainModel mainModel)
			: base("EnableOnStartCommand", "_Enable on start.", typeof(EnableOnStartCommand))
		{
			model = mainModel;
			model.IsEnabledOnStart = model.Logic.ConfigurationLogic.Section.EnableOnStart;
		}

		public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			model.Logic.ConfigurationLogic.Section.EnableOnStart = checkBox.IsChecked ?? false;
		}
	}
}