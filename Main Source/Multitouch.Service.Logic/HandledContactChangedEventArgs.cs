using System;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic
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