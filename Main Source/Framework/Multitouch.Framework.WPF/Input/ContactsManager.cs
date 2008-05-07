using System;
using System.Collections.Generic;

namespace Multitouch.Framework.WPF.Input
{
	class ContactsManager
	{
		static ContactsManager instance;
		static object lockInstance = new object();

		public ContactsManager()
		{
			AllContacts = new Dictionary<int, Contact>();
		}

		public static ContactsManager Instance
		{
			get
			{
				if(instance == null)
				{
					lock(lockInstance)
					{
						if(instance == null)
							instance = new ContactsManager();
					}
				}
				return instance;
			}
		}
		public IDictionary<int, Contact> AllContacts { get; private set; }
	}
}
