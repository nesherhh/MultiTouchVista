using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using Multitouch.Contracts.Contracts;

namespace Multitouch.Contracts.Contracts
{
	[AddInContract]
	public interface IInputPostHandlerContract : IContract
	{
		void Start();
		void Stop();
		void Handle(IntPtr windowHandle, IContactContract contact);
		int Order { get; }
	}
}
