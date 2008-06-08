using System;
using System.Collections.ObjectModel;

namespace DWMaxxAddIn
{
	class WindowCollection : KeyedCollection<IntPtr, Window>
	{
		protected override IntPtr GetKeyForItem(Window item)
		{
			return item.HWnd;
		}

		public bool TryGetValue(IntPtr handle, out Window SystemWindow)
		{
			SystemWindow = null;
			bool result = Contains(handle);
			if (result)
				SystemWindow = this[handle];
			return result;
		}
	}
}
