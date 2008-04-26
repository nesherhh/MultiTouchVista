using System;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	public class ContactEventArgs : InputEventArgs
	{
		public Contact Contact { get; private set; }
		public MultitouchDevice MultitouchDevice { get; private set; }

		internal ContactEventArgs(MultitouchDevice device, RawMultitouchReport raw, int timestamp)
			: this(device, new Contact(raw.Id, raw.X, raw.Y, raw.Width, raw.Height, raw.State), timestamp)
		{ }

		internal ContactEventArgs(MultitouchDevice device, Contact contact, int timestamp)
			: base(device, timestamp)
		{
			Contact = contact;
			MultitouchDevice = device;
		}
	}
}