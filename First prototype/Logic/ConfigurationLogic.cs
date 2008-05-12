using System;
using System.Configuration;
using Danilins.Multitouch.Logic.Configuration;

namespace Danilins.Multitouch.Logic
{
	public class ConfigurationLogic : Common.Logics.Logic
	{
		private System.Configuration.Configuration configuration;
		private MultitouchSection multitouchSection;

		const string MULTITOUCH_SECTION = "multitouchSection";

		public ConfigurationLogic(IServiceProvider parentProvider)
			: base(parentProvider)
		{
			configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			multitouchSection = (MultitouchSection)configuration.GetSection(MULTITOUCH_SECTION);
			if (multitouchSection == null)
			{
				multitouchSection = new MultitouchSection();
				AddSection(MULTITOUCH_SECTION, multitouchSection);
			}
		}

		public MultitouchSection Section
		{
			get { return multitouchSection; }
		}

		public T GetSection<T>(string name) where T: ConfigurationSection
		{
			return (T)configuration.GetSection(name);
		}

		public void Save()
		{
			configuration.Save();
		}

		public void AddSection(string name, ConfigurationSection section)
		{
			configuration.Sections.Add(name, section);
		}
	}
}