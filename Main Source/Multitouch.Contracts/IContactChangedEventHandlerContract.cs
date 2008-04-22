using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	[EventHandler]
	public interface IContactChangedEventHandlerContract : IContract
	{
		void Handler(IContactChangedEventArgsContract args);
	}
}
