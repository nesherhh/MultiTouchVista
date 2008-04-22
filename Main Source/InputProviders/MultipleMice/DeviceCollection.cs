using System;
using System.Collections.ObjectModel;

namespace MultipleMice
{
	class DeviceCollection : KeyedCollection<IntPtr, DeviceState>
	{
		protected override IntPtr GetKeyForItem(DeviceState item)
		{
			return item.Handle;
		}
	}
}
