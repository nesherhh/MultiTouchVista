using System;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// 
	/// </summary>
	public class ContactEventArgs : EventArgs
	{
		/// <summary>
		/// Contact
		/// </summary>
		/// <value>The contact.</value>
		public Contact Contact { get; private set; }

		internal ContactEventArgs(ContactData contact, long timestamp)
		{
			Contact = new Contact(contact, timestamp);
		}
	}
}
