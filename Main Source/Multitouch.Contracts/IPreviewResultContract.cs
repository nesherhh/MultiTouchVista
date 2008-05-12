using System;
using System.AddIn.Contract;
using Multitouch.Contracts.Contracts;

namespace Multitouch.Contracts
{
	public interface IPreviewResultContract : IContract
	{
		IContactContract Contact { get; }
		IntPtr HWnd { get; }
		bool Handled { get; }
	}
}
