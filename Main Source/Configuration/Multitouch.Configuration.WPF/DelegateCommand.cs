using System;
using System.Windows.Input;

namespace Multitouch.Configuration.WPF
{
	class DelegateCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		readonly Action<object> onExecute;
		readonly Func<object, bool> onCanExecute;

		public DelegateCommand(Action<object> execute)
			: this(execute, null)
		{ }

		public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
		{
			Check.NotNull(execute, "execute");
			onExecute = execute;
			onCanExecute = canExecute;
		}

		public void Execute(object parameter)
		{
			onExecute(parameter);
		}

		public bool CanExecute(object parameter)
		{
			if (onCanExecute != null)
				return onCanExecute(parameter);
			return true;
		}

		public void RaiseCanExecuteChanged()
		{
			EventHandler canExecuteChanged = CanExecuteChanged;
			if (canExecuteChanged != null)
				canExecuteChanged(this, EventArgs.Empty);
		}
	}
}