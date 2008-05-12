using System;
using System.Windows;
using System.Windows.Input;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Framework.Input
{
	class MouseHandler : UIElement, IDisposable
	{
		MultitouchScreen screen;
		int contactId = 1;
		ContactInfo lastContact;

		public MouseHandler(MultitouchScreen screen)
		{
			this.screen = screen;
			Application.Current.Activated += Current_Activated;
			Application.Current.Deactivated += Current_Deactivated;

			if (Application.Current.MainWindow != null)
				Current_Activated(null, EventArgs.Empty);
		}

		public void Dispose()
		{
			Application.Current.Deactivated -= Current_Deactivated;
			Application.Current.Activated -= Current_Activated;
			Unsubscribe();
		}

		void Current_Deactivated(object sender, EventArgs e)
		{
			Unsubscribe();
		}

		public void Unsubscribe()
		{
			Window window = Application.Current.MainWindow;
			if (window != null)
			{
				window.MouseLeftButtonDown -= MouseDownHandler;
				window.MouseLeftButtonUp -= MouseUpHandler;
				window.MouseMove -= MouseMoveHandler;
			}
		}

		void Current_Activated(object sender, EventArgs e)
		{
			Subscribe();
		}

		public void Subscribe()
		{
			Window window = Application.Current.MainWindow;
			window.MouseLeftButtonDown += MouseDownHandler;
			window.MouseLeftButtonUp += MouseUpHandler;
			window.MouseMove += MouseMoveHandler;
		}

		private void MouseUpHandler(object sender, MouseButtonEventArgs e)
		{
			if (lastContact != null)
			{
				Point position = e.GetPosition(Application.Current.MainWindow);
				position = Application.Current.MainWindow.PointToScreen(position);

				ContactInfo contact = new ContactInfo(new Rect(position, new Size(10, 10)));
				contact.Id = lastContact.Id;
				contact.State = ContactState.Up;
				contact.Delta = contact.Center - lastContact.Center;
				contact.DeltaArea = contact.Area - lastContact.Area;
				contact.PredictedPos = contact.Center + contact.Delta;
				contact.Displacement = lastContact.Displacement + contact.Delta;

				lastContact = null;

				ProcessContact(contact);
			}
		}

		private void MouseMoveHandler(object sender, MouseEventArgs e)
		{
			if (lastContact != null)
			{
				Point position = e.GetPosition(Application.Current.MainWindow);
				position = Application.Current.MainWindow.PointToScreen(position);

				ContactInfo contact = new ContactInfo(new Rect(position, new Size(10, 10)));
				contact.Id = lastContact.Id;
				contact.State = ContactState.Move;
				contact.Delta = contact.Center - lastContact.Center;
				contact.DeltaArea = contact.Area - lastContact.Area;
				contact.PredictedPos = contact.Center + contact.Delta;
				contact.Displacement = lastContact.Displacement + contact.Delta;

				lastContact = contact;

				ProcessContact(contact);
			}
		}

		private void MouseDownHandler(object sender, MouseButtonEventArgs e)
		{
			Point position = e.GetPosition(Application.Current.MainWindow);
			position = Application.Current.MainWindow.PointToScreen(position);

			ContactInfo contact = new ContactInfo(new Rect(position, new Size(10, 10)));
			contact.State = ContactState.Down;
			contact.Id = contactId++;
			if (contactId > 65565)
				contactId = 1;
			contact.PredictedPos = contact.Center;

			lastContact = contact;

			ProcessContact(contact);
		}

		void ProcessContact(ContactInfo contact)
		{
			screen.ProcessContacts(new[] {contact});
		}
	}
}