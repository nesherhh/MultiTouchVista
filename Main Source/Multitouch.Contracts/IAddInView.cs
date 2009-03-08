using System;

namespace Multitouch.Contracts
{
	public interface IAddInView
	{
		string Id { get; set; }
		string Publisher { get; set; }
		string Description { get; set; }
		string Version { get; set; }
	}
}
