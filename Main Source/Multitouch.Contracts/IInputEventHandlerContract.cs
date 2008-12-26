using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	[EventHandler]
	public interface IInputEventHandlerContract : IContract
	{
		void Handler(IInputDataEventArgsContract e);
	}
}
