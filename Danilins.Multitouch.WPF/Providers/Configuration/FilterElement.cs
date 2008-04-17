using System;
using System.ComponentModel;
using System.Configuration;

namespace Danilins.Multitouch.Providers.Configuration
{
	internal class FilterElement:ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey=true, IsRequired=true)]
		public string Name
		{
			get { return (string) this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("type", IsRequired=true), TypeConverter(typeof(TypeNameConverter))]
		public Type Type
		{
			get { return (Type) this["type"]; }
			set { this["type"] = value; }
		}

		[ConfigurationProperty("parameters")]
		public FilterParameterCollection Parameters
		{
			get { return (FilterParameterCollection) base["parameters"]; }
		}
	}
}