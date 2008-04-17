using System;

namespace Danilins.Multitouch.Common.Providers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class InputProviderAttribute : Attribute
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public bool HasConfiguration { get; private set; }

		public InputProviderAttribute(string id, string name, bool hasConfiguration)
		{
			Id = new Guid(id);
			Name = name;
			HasConfiguration = hasConfiguration;
		}
	}
}