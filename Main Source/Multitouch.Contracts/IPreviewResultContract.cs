using System;
using System.AddIn.Contract;

namespace Multitouch.Contracts.Contracts
{
	public interface IPreviewResultContract : IContract
	{
		IContactContract Contact { get; }
		IntPtr HWnd { get; }
		bool Handled { get; }
	}
}
