using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using Multitouch.Contracts.Contracts;

namespace Multitouch.Contracts.Contracts
{
	[AddInContract]
	public interface IInputPostHandlerContract : IContract
	{
		void Handle(IContactContract contact);
	}
}
