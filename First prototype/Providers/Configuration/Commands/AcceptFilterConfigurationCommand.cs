using System;
using System.Windows.Input;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.Providers.Configuration.Models;

namespace Danilins.Multitouch.Providers.Configuration.Commands
{
	class AcceptFilterConfigurationCommand:CommandModel
	{
		FilterConfigurationModel model;

		public AcceptFilterConfigurationCommand(FilterConfigurationModel model)
			: base("AcceptFilterConfiguration", "OK", typeof(AcceptFilterConfigurationCommand))
		{
			this.model = model;
		}

		public override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			model.Save();
		}
	}
}
