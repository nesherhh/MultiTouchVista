using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	[EventHandler]
	public interface INewFrameEventHandlerContract : IContract
	{
		void Handler(INewFrameEventArgsContract e);
	}
}
