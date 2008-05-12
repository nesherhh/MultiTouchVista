using System;
using System.Configuration;

namespace Danilins.Multitouch.Providers.Configuration
{
	internal class FilterCollection : ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new FilterElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FilterElement)element).Name;
		}

		public FilterElement this[int index]
		{
			get { return (FilterElement)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		public new FilterElement this[string name]
		{
			get { return (FilterElement)BaseGet(name); }
		}

		public void Add(FilterElement item)
		{
			BaseAdd(item);
		}

		public int IndexOf(FilterElement item)
		{
			return BaseIndexOf(item);
		}

		public void Remove(FilterElement item)
		{
			if(IndexOf(item) > 0)
				BaseRemove(item.Name);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Clear()
		{
			BaseClear();
		}
	}
}