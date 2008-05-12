using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace Multitouch.Contracts.Contracts
{
	[AddInContract]
	public interface IInputPreviewHandlerContract : IContract
	{
		IPreviewResultContract Handle(IContactContract contact);
	}
}
