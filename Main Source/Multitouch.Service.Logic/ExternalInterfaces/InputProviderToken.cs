using System;
using System.Runtime.Serialization;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[DataContract]
	public class InputProviderToken
	{
		public InputProviderToken(IAddInView token)
		{
			Name = token.Id;
			Description = token.Description;
			Publisher = token.Publisher;
			Version = token.Version;
		}

		[DataMember(IsRequired = true)]
		public string Name { get; private set; }
		[DataMember(IsRequired = true)]
		public string Description { get; private set; }
		[DataMember(IsRequired = true)]
		public string Publisher { get; private set; }
		[DataMember(IsRequired = true)]
		public string Version { get; private set; }
	}
}