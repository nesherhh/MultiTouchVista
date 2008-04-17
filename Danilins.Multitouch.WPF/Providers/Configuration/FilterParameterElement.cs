using System;
using System.ComponentModel;
using System.Configuration;

namespace Danilins.Multitouch.Providers.Configuration
{
	internal class FilterParameterElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("value", IsRequired = true)]
		public string Value
		{
			get { return (string)this["value"]; }
			set { this["value"] = value; }
		}

		[ConfigurationProperty("type", IsRequired = true), TypeConverter(typeof(TypeNameConverter))]
		public Type Type
		{
			get { return (Type) this["type"]; }
			set { this["type"] = value; }
		}
	}
}