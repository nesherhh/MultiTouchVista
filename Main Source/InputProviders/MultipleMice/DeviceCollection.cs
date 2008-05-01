using System;
using System.Collections.ObjectModel;

namespace MultipleMice
{
	class DeviceCollection : KeyedCollection<IntPtr, DeviceStatus>
	{
		protected override IntPtr GetKeyForItem(DeviceStatus item)
		{
			return item.Handle;
		}
	}
}
