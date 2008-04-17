using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Framework
{
	public class ContactEventArgs : InputEventArgs
	{
		internal ContactEventArgs(MultitouchDevice device, ContactInfo contact)
			: base(device, 0)
		{
			Contact = contact;
		}

		public ContactInfo Contact { get; private set; }

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			ContactEventHandler handler = (ContactEventHandler)genericHandler;
			handler(genericTarget, this);
		}

		public Point GetPosition(Visual element)
		{
			return element.PointFromScreen(Contact.Center);
		}
	}
}