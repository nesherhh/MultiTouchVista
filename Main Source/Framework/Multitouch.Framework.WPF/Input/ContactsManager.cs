using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Multitouch.Framework.WPF.Input
{
	class ContactsManager
	{
		readonly Dispatcher dispatcher;
		public IDictionary<int, Contact> ExistingContacts { get; private set; }

		public ContactsManager(Dispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			ExistingContacts = new Dictionary<int, Contact>();
		}

		public IEnumerable<Contact> GetContacts(Func<Contact, bool> predicate)
		{
			return ExistingContacts.Values.Where(predicate);
		}

		public IEnumerable<Contact> GetContactsCaptured(IInputElement element)
		{
			return GetContacts(c => element == c.InputArgs.Captured);
		}

		public bool Capture(Contact contact, IInputElement element, CaptureMode captureMode)
		{
			if(element == null)
				captureMode = CaptureMode.None;
			if(captureMode == CaptureMode.None)
				element = null;

			DependencyObject oldCaptured = contact.InputArgs.Captured;
			if(oldCaptured == element)
				return true;

			using (dispatcher.DisableProcessing())
			{
				long timestamp = Stopwatch.GetTimestamp();
				if (contact.InputArgs.Captured != null)
					RaiseCaptureEvent(contact, MultitouchScreen.LostContactCaptureEvent, contact.InputArgs.Captured, timestamp);


				contact.InputArgs.Captured = (DependencyObject)element;
				contact.InputArgs.CaptureState = captureMode;

				if (contact.InputArgs.Captured != null)
					RaiseCaptureEvent(contact, MultitouchScreen.GotContactCaptureEvent, contact.InputArgs.Captured, timestamp);
			}
			return true;
		}

		static void RaiseCaptureEvent(Contact contact, RoutedEvent routedEvent, DependencyObject source, long timestamp)
		{
			ContactEventArgs args = new ContactEventArgs(contact, timestamp);
			args.RoutedEvent = routedEvent;
			args.Source = source;
			MultitouchLogic.RaiseEvent(source, args);
		}
	}
}
