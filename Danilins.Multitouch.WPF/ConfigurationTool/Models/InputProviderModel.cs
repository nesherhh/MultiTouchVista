using System;

namespace Danilins.Multitouch.ConfigurationTool.Models
{
	public class InputProviderModel
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public bool HasConfiguration { get; private set; }

		public InputProviderModel(Guid id, string name, bool hasConfiguration)
		{
			Id = id;
			Name = name;
			HasConfiguration = hasConfiguration;
		}
	}
}
