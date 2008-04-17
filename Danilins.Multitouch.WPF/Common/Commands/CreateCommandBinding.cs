using System;
using System.Windows;
using System.Windows.Input;

namespace Danilins.Multitouch.Common.Commands
{
	public static class CreateCommandBinding
	{
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(CommandModel), typeof(CreateCommandBinding),
			                                    new PropertyMetadata(new PropertyChangedCallback(OnCommandInvalidated)));
		
		[AttachedPropertyBrowsableForType(typeof(ICommand))]
		public static CommandModel GetCommand(DependencyObject sender)
		{
			return (CommandModel)sender.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject sender, CommandModel command)
		{
			sender.SetValue(CommandProperty, command);
		}

		private static void OnCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			UIElement element = (UIElement)dependencyObject;
			element.CommandBindings.Clear();

			CommandModel commandModel = e.NewValue as CommandModel;
			if (commandModel != null)
				element.CommandBindings.Add(new CommandBinding(commandModel.Command, commandModel.OnExecute, commandModel.OnQueryEnabled));

			CommandManager.InvalidateRequerySuggested();
		}
	}
}