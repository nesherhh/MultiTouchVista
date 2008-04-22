using System;
using System.Collections.ObjectModel;

namespace MultipleMice
{
	class ContactCollection : KeyedCollection<IntPtr, MouseContact>
	{
		protected override IntPtr GetKeyForItem(MouseContact item)
		{
			return item.Handle;
		}
	}
}
