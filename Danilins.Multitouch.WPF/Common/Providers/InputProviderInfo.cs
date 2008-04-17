using System;

namespace Danilins.Multitouch.Common.Providers
{
	public class InputProviderInfo
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public Type Type { get; private set; }
		public bool HasConfiguration { get; private set; }

		public InputProviderInfo(Guid id, string name, Type type, bool hasConfiguration)
		{
			Id = id;
			Name = name;
			Type = type;
			HasConfiguration = hasConfiguration;
		}
	}
}