using System;
using System.AddIn.Contract;

namespace Multitouch.Contracts.Contracts
{
	public interface IFrameDataContract : IContract
	{
		IListContract<IImageDataContract> Image { get; }
	}
}
