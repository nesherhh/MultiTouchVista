using System;
using Multitouch.Contracts;

namespace MultipleMice
{
	class MouseContactChangedEventArgs : ContactChangedEventArgs
	{
		IContact contact;

		public MouseContactChangedEventArgs(MouseContact contact)
		{
			this.contact = contact;
		}

		public override IContact Contact
		{
			get { return contact; }
		}
	}
}
