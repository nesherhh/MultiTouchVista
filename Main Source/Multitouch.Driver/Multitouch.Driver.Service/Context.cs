using System;
using System.Windows.Forms;
using Multitouch.Driver.Logic;
using Multitouch.Service.Logic;

namespace Multitouch.Driver.Service
{
	class Context : ApplicationContext
	{
		MultitouchInput input;
		MultitouchDriver driver;

		public Context()
		{
			input = new MultitouchInput();
			input.Start();

			driver = new MultitouchDriver();
			driver.Start();
		}

		protected override void ExitThreadCore()
		{
			if (input != null)
			{
				input.Stop();
				input = null;
			}
			if (driver != null)
			{
				driver.Stop();
				driver = null;
			}
			base.ExitThreadCore();
		}
	}
}