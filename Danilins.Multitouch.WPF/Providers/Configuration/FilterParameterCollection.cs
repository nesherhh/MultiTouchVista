using System;
using System.Configuration;

namespace Danilins.Multitouch.Providers.Configuration
{
	internal class FilterParameterCollection:ConfigurationElementCollection
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new FilterParameterElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((FilterParameterElement) element).Name;
		}

		public FilterParameterElement this[int index]
		{
			get { return (FilterParameterElement)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		public new FilterParameterElement this[string name]
		{
			get { return (FilterParameterElement)BaseGet(name); }
		}

		public void Add(FilterParameterElement item)
		{
			BaseAdd(item);
		}

		public int IndexOf(FilterParameterElement item)
		{
			return BaseIndexOf(item);
		}

		public void Remove(FilterParameterElement item)
		{
			if (IndexOf(item) > 0)
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