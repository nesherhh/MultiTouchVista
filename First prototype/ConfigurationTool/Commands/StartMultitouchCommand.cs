using System;
using System.Windows.Input;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.Logic;
using Danilins.Multitouch.ConfigurationTool.Models;

namespace Danilins.Multitouch.ConfigurationTool.Commands
{
	class StartMultitouchCommand:CommandModel
	{
		MainModel mainModel;

		public StartMultitouchCommand(MainModel model) : base("StartMultitouchCommand", "_Start Multitouch", typeof(StartMultitouchCommand))
		{
			mainModel = model;
		}

		public override void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsRunning;
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
			if(!IsRunning)
				StartService();
		}

		internal void StartService()
		{
			InputProviderLogic providerLogic = mainModel.Logic.InputProviderLogic;
			//providerLogic.PreviewInputHandler = mainModel.InputHandler;
			//providerLogic.PreviewOutputHandler = mainModel.OutputHandler;

			providerLogic.Start();
			mainModel.Logic.ServiceLogic.Start();
			mainModel.IsMultitouchRunning = true;
		}
	}
}
