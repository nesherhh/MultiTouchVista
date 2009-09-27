using System;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Framework.Service;

namespace Danilins.Multitouch.Framework
{
	class MultitouchServiceCallbackHandler:IMultitouchServiceCallback
	{
		private readonly MultitouchScreen screen;

		public MultitouchServiceCallbackHandler(MultitouchScreen screen)
		{
			this.screen = screen;
		}

		public void ProcessContact(ContactInfo[] contact)
		{
			screen.ProcessContacts(contact);
		}
	}
}
