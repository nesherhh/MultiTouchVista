using System;
using System.Collections.ObjectModel;

namespace DWMaxxAddIn
{
	class WindowContacts : KeyedCollection<int, WindowContact>
	{
		readonly Window owner;

		public WindowContacts(Window owner)
		{
			this.owner = owner;
		}

		protected override int GetKeyForItem(WindowContact item)
		{
			return item.Id;
		}

		protected override void InsertItem(int index, WindowContact item)
		{
			item.SetOwner(owner);
			base.InsertItem(index, item);
		}

		public bool TryGetValue(int id, out WindowContact contact)
		{
			if (Contains(id))
			{
				contact = this[id];
				return true;
			}
			contact = null;
			return false;
		}
	}
}
