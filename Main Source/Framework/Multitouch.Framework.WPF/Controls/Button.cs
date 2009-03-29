using System;
using System.Linq;
using System.Windows.Controls;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls
{
	/// <summary>
	/// Extends <see cref="System.Windows.Controls.Button"/> to receive Multitouch events.
	/// </summary>
	public class Button : System.Windows.Controls.Button
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Button"/> class.
		/// </summary>
		public Button()
		{
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
					bool shouldMakeClick = IsPressed && ClickMode == ClickMode.Release;
					e.Contact.ReleaseCapture();
					if (MultitouchScreen.GetContactsCaptured(this).Count() == 0)
					{
						if(shouldMakeClick)
							OnClick();
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
							OnClick();
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
	}
}