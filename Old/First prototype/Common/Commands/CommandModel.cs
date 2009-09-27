using System;
using System.Windows.Input;

namespace Danilins.Multitouch.Common.Commands
{
	public abstract class CommandModel
	{
		private RoutedUICommand routedCommand;

		public CommandModel(string name, string text, Type ownerType)
		{
			routedCommand = new RoutedUICommand(text, name, ownerType);
		}

		public RoutedUICommand Command
		{
			get { return routedCommand; }
		}

		public virtual void OnQueryEnabled(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
			e.Handled = true;
		}

		public abstract void OnExecute(object sender, ExecutedRoutedEventArgs e);
	}
}