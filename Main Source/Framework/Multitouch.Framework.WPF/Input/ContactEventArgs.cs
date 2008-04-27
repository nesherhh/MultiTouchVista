using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using Multitouch.Framework.Collections;

namespace Multitouch.Framework.WPF.Input
{
	public class ContactEventArgs : InputEventArgs
	{
		public Contact Contact { get; private set; }
		internal MultitouchDevice MultitouchDevice { get; private set; }

		internal ContactEventArgs(MultitouchDevice device, RawMultitouchReport raw, int timestamp)
			: this(device, raw.Contact, timestamp)
		{ }

		internal ContactEventArgs(MultitouchDevice device, Contact contact, int timestamp)
			: base(device, timestamp)
		{
			Contact = contact;
			MultitouchDevice = device;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			ContactEventHandler handler = (ContactEventHandler)genericHandler;
			handler(genericTarget, this);
		}

		public Point GetPosition(IInputElement relativeTo)
		{
			return MultitouchDevice.GetPosition(relativeTo);
		}

		public IDictionary<int, Contact> AllContacts
		{
			get { return new ReadOnlyDictionary<int, Contact>(MultitouchDevice.AllContacts); }
		}

		public IDictionary<int, Contact> GetContacts(UIElement overElement, MatchCriteria criteria)
		{
			return new ReadOnlyDictionary<int, Contact>(MultitouchDevice.GetContacts(overElement, criteria));
		}
	}
}