using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	[EventArgs]
	public interface IContactChangedEventArgsContract : IContract
	{
		[Comment(
		"/// <summary>"+
		"/// Changed contact."+
		"/// </summary>"
		)]
		IContactContract Contact { get; }
	}
}
