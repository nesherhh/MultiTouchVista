using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Danilins.Multitouch.Controls
{
	public class ContactInfoModelCollection:KeyedCollection<int, ContactInfoModel>
	{
		protected override int GetKeyForItem(ContactInfoModel item)
		{
			return item.Id;
		}
	}
}
