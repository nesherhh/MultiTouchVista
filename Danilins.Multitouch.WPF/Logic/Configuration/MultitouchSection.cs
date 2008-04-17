using System;
using System.Configuration;

namespace Danilins.Multitouch.Logic.Configuration
{
	public class MultitouchSection : ConfigurationSection
	{
		[ConfigurationProperty("inputProvider")]
		public Guid InputProvider
		{
			get { return (Guid)this["inputProvider"]; }
			set { this["inputProvider"] = value; }
		}

		[ConfigurationProperty("enableOnStart")]
		public bool EnableOnStart
		{
			get { return (bool)this["enableOnStart"]; }
			set { this["enableOnStart"] = value; }
		}
	}
}