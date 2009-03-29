using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Controls.Primitives
{
	/// <summary>
	/// Extends <see cref="System.Windows.Controls.Primitives.Thumb"/> to receive Multitouch events.
	/// </summary>
	public class Thumb : System.Windows.Controls.Primitives.Thumb
	{
		Point originalThumbPosition;
		Point lastThumbPosition;
		Vector screenTotalDelta;

		/// <summary>
		/// Initializes a new instance of the <see cref="Thumb"/> class.
		/// </summary>
		public Thumb()
		{
			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);
			AddHandler(MultitouchScreen.LostContactCaptureEvent, (ContactEventHandler)OnLostContactCapture);
			AddHandler(MultitouchScreen.GotContactCaptureEvent, (ContactEventHandler)OnGotContactCapture);
		}

		void OnGotContactCapture(object sender, ContactEventArgs e)
		{
			if (!IsDragging)
			{
				IsDragging = true;

				screenTotalDelta = new Vector();
				originalThumbPosition = e.GetPosition(this);
				lastThumbPosition = originalThumbPosition;

				bool failed = true;
				try
				{
					RaiseEvent(new DragStartedEventArgs(originalThumbPosition.X, originalThumbPosition.Y));
					failed = false;
				}
				finally
				{
					if (failed)
						CancelDrag();
				}
			}
		}

		void OnLostContactCapture(object sender, ContactEventArgs e)
		{
			IsDragging = false;

			RaiseEvent(new DragCompletedEventArgs(screenTotalDelta.X, screenTotalDelta.Y, false));
			originalThumbPosition.X = 0.0;
			originalThumbPosition.Y = 0.0;
		}

		void OnContactMoved(object sender, ContactEventArgs e)
		{
			if(IsDragging)
			{
				Point currentPosition = e.GetPosition(this);
				if (currentPosition != lastThumbPosition)
				{
					e.Handled = true;
					RaiseEvent(new DragDeltaEventArgs(currentPosition.X - lastThumbPosition.X, currentPosition.Y - lastThumbPosition.Y));
					lastThumbPosition = currentPosition;
				}
			}
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			e.Handled = true;
			if (e.Contact.Captured == this)
				e.Contact.Capture(null);
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			e.Handled = true;
			e.Contact.Capture(this);
		}
	}
}
