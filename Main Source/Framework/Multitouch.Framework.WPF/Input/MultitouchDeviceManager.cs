using System;
using System.Collections.Generic;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	class MultitouchDeviceManager
	{
		Dictionary<int, MultitouchDevice> deviceList;

		public MultitouchDeviceManager()
		{
			deviceList = new Dictionary<int, MultitouchDevice>();
		}

		public MultitouchDevice GetDevice(int id, ContactState state)
		{
			MultitouchDevice result;
			switch (state)
			{
				case ContactState.New:
					CreateDevice(id);
					result = GetDevice(id);
					break;
				case ContactState.Removed:
					result = GetDevice(id);
					RemoveDevice(id);
					break;
				case ContactState.Moved:
					result = GetDevice(id);
					break;
				default:
					throw new ArgumentOutOfRangeException("state");
			}
			return result;
		}

		MultitouchDevice GetDevice(int id)
		{
			if (deviceList.ContainsKey(id))
				return deviceList[id];
			return null;
		}

		void RemoveDevice(int id)
		{
			deviceList.Remove(id);
		}

		void CreateDevice(int id)
		{
			deviceList.Add(id, new MultitouchDevice());
		}
	}
}