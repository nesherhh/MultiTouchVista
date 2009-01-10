using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	[EventArgs]
	public interface INewFrameEventArgsContract : IContract
	{
		IListContract<IImageDataContract> Images { get; }
		IListContract<IContactDataContract> Contacts { get; }
		long Timestamp { get; }
	}
}