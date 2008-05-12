using System;
using System.Windows.Input;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.ConfigurationTool.Models;

namespace Danilins.Multitouch.ConfigurationTool.Commands
{
	class StopMultitouchCommand : CommandModel
	{
		MainModel mainModel;

		public StopMultitouchCommand(MainModel model)
			: base("StopMultitouchCommand", "Sto_p Multitouch", typeof(StopMultitouchCommand))
		{
			mainModel = model;
		}

		public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsRunning;
		}

		private bool IsRunning
		{
			get
			{
				if (!ViewUtility.IsDesignTime)
					return mainModel.Logic.InputProviderLogic.IsRunning;
				return false;
			}
		}

		public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			if (IsRunning)
				StopService();
		}

		internal void StopService()
		{
			mainModel.Logic.ServiceLogic.Stop();
			mainModel.Logic.InputProviderLogic.Stop();
			mainModel.IsMultitouchRunning = false;
		}
	}
}