using System;
using System.Collections.ObjectModel;

namespace Danilins.Multitouch.Common.Providers
{
	public class InputProviderCollection:KeyedCollection<Guid, InputProviderInfo>
	{
		protected override Guid GetKeyForItem(InputProviderInfo item)
		{
			return item.Id;
		}
	}
}
