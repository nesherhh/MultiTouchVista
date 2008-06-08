using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace Multitouch.Contracts.Contracts
{
	[AddInContract]
	public interface IInputPreviewHandlerContract : IContract
	{
		void Start();
		void Stop();
		IPreviewResultContract Handle(IContactContract contact);
		int Order { get; }
	}
}
