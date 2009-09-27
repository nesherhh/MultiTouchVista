using System;
using System.Collections.ObjectModel;

namespace Danilins.Multitouch.Logic.LegacySupport
{
	class ContactDataCollection:KeyedCollection<int, ContactData>
	{
		protected override int GetKeyForItem(ContactData item)
		{
			return item.Id;
		}
	}
}
