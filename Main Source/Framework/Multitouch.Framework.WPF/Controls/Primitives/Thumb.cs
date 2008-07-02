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
		Point originThumbPoint;
		Point previousScrennCoordPosition;
		Point originScreenCoordPosition;

		/// <summary>
		/// Initializes a new instance of the <see cref="Thumb"/> class.
		/// </summary>
		public Thumb()
		{
			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);
		}

		void OnContactMoved(object sender, ContactEventArgs e)
		{
			if(IsDragging)
			{
				Point position = e.GetPosition(this);
				Point point = PointToScreen(position);
				if(point != previousScrennCoordPosition)
				{
					previousScrennCoordPosition = point;
					e.Handled = true;
					RaiseEvent(new DragDeltaEventArgs(position.X - originThumbPoint.X, position.Y - originThumbPoint.Y));
				}
			}
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			if(e.Captured != this && IsDragging)
			{
				e.Handled = true;
				IsDragging = false;
				e.MultitouchDevice.Capture(null);

				Point point = PointToScreen(e.GetPosition(this));
				RaiseEvent(new DragCompletedEventArgs(point.X - originScreenCoordPosition.X, point.Y - originScreenCoordPosition.Y, false));

				originThumbPoint.X = 0.0;
				originThumbPoint.Y = 0.0;
			}
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			if(!IsDragging)
			{
				e.Handled = true;
				e.MultitouchDevice.Capture(this);
				IsDragging = true;

				originThumbPoint = e.GetPosition(this);
				previousScrennCoordPosition = originScreenCoordPosition = PointToScreen(originThumbPoint);

				bool ok = false;
				try
				{
					RaiseEvent(new DragStartedEventArgs(originThumbPoint.X, originThumbPoint.Y));
					ok = true;
				}
				finally
				{
					if(!ok)
						CancelDrag();
				}
			}
		}
	}
}
