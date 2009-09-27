using System;
using System.Configuration;
using Danilins.Multitouch.Providers.Configuration;

namespace Danilins.Multitouch.Providers.Configuration
{
	internal class FiltersSection : ConfigurationSection
	{
		[ConfigurationProperty("filters")]
		[ConfigurationCollection(typeof(FilterElement), AddItemName = "addFilter", RemoveItemName = "removeFilter", ClearItemsName = "clearFilters")]
		public FilterCollection Filters
		{
			get { return (FilterCollection)base["filters"]; }
		}
	}
}