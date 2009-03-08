using System;
using Microsoft.Practices.Composite.Presentation.Commands;

namespace Multitouch.Configuration.WPF
{
	class DelegateCommand : DelegateCommand<object>
	{
		public DelegateCommand(Action<object> executeMethod)
			: base(executeMethod)
		{ }

		public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
			: base(executeMethod, canExecuteMethod)
		{ }
	}
}