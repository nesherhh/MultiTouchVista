using System;
using System.AddIn.Hosting;
using System.Runtime.Serialization;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[DataContract]
	public class InputProviderToken
	{
		public InputProviderToken(AddInToken token)
		{
			AddInFullName = token.AddInFullName;
			Description = token.Description;
			Name = token.Name;
			Publisher = token.Publisher;
			Version = token.Version;
			AssemblyName = token.AssemblyName.FullName;
		}

		[DataMember(IsRequired = true)]
		public string AddInFullName { get; private set; }
		[DataMember(IsRequired = true)]
		public string Description { get; private set; }
		[DataMember(IsRequired = true)]
		public string Name { get; private set; }
		[DataMember(IsRequired = true)]
		public string Publisher { get; private set; }
		[DataMember(IsRequired = true)]
		public string Version { get; private set; }
		[DataMember(IsRequired = true)]
		public string AssemblyName { get; private set; }
	}
}