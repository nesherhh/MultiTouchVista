using System;
using Multitouch.Contracts;

namespace Multitouch.Service
{
	class HandledContactChangedEventArgs : ContactChangedEventArgs
	{
		IContact contact;

		internal HandledContactChangedEventArgs(IContact contact)
		{
			this.contact = contact;
		}

		public override IContact Contact
		{
			get { return contact; }
		}
	}
}
