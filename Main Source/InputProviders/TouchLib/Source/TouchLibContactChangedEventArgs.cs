using System;
using Multitouch.Contracts;

namespace TouchLib
{
	class TouchLibContactChangedEventArgs : ContactChangedEventArgs
	{
		IContact contact;

		public override IContact Contact
		{
			get { return contact; }
		}

		public TouchLibContactChangedEventArgs(IContact contact)
		{
			this.contact = contact;
		}
	}
}