using System;
using System.Linq;
using System.Windows.Controls;
using Multitouch.Framework.WPF.Input;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Controls.Primitives
{
	/// <summary>
	/// Extends <see cref="System.Windows.Controls.Primitives.RepeatButton"/> to support Multitouch events.
	/// </summary>
	public class RepeatButton : System.Windows.Controls.Primitives.RepeatButton
	{
		readonly Proc<System.Windows.Controls.Primitives.RepeatButton> startTimerMethod;
		readonly Proc<System.Windows.Controls.Primitives.RepeatButton> stopTimerMethod;

		/// <summary>
		/// Initializes a new instance of the <see cref="RepeatButton"/> class.
		/// </summary>
		public RepeatButton()
		{
			startTimerMethod = Dynamic<System.Windows.Controls.Primitives.RepeatButton>.Instance.Procedure.Explicit.CreateDelegate("StartTimer");
			stopTimerMethod = Dynamic<System.Windows.Controls.Primitives.RepeatButton>.Instance.Procedure.Explicit.CreateDelegate("StopTimer");

			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			if (ClickMode != ClickMode.Hover)
			{
				e.Handled = true;
				if (e.Contact.Captured == this)
				{
					e.Contact.ReleaseCapture();
					if (MultitouchScreen.GetContactsCaptured(this).Count() == 0)
					{
						StopTimer();
						IsPressed = false;
					}
				}
			}
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			if (ClickMode != ClickMode.Hover)
			{
				e.Handled = true;
				if (e.Contact.Capture(this))
				{
					IsPressed = true;
					if (ClickMode == ClickMode.Press && MultitouchScreen.GetContactsCaptured(this).Count() == 1)
					{
						bool failed = true;
						try
						{
							StartTimer();
							failed = false;
						}
						finally
						{
							if (failed)
							{
								IsPressed = false;
								e.Contact.ReleaseCapture();
							}
						}
					}
				}
			}
		}

		void StartTimer()
		{
			startTimerMethod(this);
		}

		void StopTimer()
		{
			stopTimerMethod(this);
		}
	}
}
