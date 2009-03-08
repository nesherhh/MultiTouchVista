using System;
using System.ComponentModel.Composition;

namespace Multitouch.Contracts
{
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class)]
	public class AddInAttribute : Attribute, IAddInView
	{
		public string Id { get; set; }
		public string Publisher { get; set; }
		public string Description { get; set; }
		public string Version { get; set; }

		public AddInAttribute(string id)
		{
			Id = id;
		}
	}
}
