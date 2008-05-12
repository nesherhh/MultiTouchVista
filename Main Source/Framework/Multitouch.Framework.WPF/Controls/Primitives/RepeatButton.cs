using System;
using System.Windows.Controls;
using Multitouch.Framework.WPF.Input;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Controls.Primitives
{
	public class RepeatButton : System.Windows.Controls.Primitives.RepeatButton
	{
		Proc<System.Windows.Controls.Primitives.RepeatButton> startTimerMethod;
		Proc<System.Windows.Controls.Primitives.RepeatButton> stopTimerMethod;

		public RepeatButton()
		{
			startTimerMethod = Dynamic<System.Windows.Controls.Primitives.RepeatButton>.Instance.Procedure.Explicit.CreateDelegate("StartTimer");
			stopTimerMethod = Dynamic<System.Windows.Controls.Primitives.RepeatButton>.Instance.Procedure.Explicit.CreateDelegate("StopTimer");

			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactLeave);
			AddHandler(MultitouchScreen.ContactEnterEvent, (ContactEventHandler)OnContactEnter);
		}

		void OnContactEnter(object sender, ContactEventArgs e)
		{
			if (e.GetContacts(this, MatchCriteria.LogicalParent).Count == 1 && !IsPressed && e.Captured == this)
				IsPressed = true;
		}

		void OnContactLeave(object sender, ContactEventArgs e)
		{
			if (e.GetContacts(this, MatchCriteria.LogicalParent).Count == 1 && IsPressed)
				IsPressed = false;
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			if (e.GetContacts(this, MatchCriteria.LogicalParent).Count == 0 && IsPressed)
			{
				IsPressed = false;
				if (ClickMode != ClickMode.Hover)
					StopTimer();
			}
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			if (e.GetContacts(this, MatchCriteria.LogicalParent).Count == 1)
			{
				IsPressed = true;
				if (ClickMode != ClickMode.Hover)
					StartTimer();
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
