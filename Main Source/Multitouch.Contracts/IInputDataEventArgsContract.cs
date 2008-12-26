using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	[EventArgs]
	public interface IInputDataEventArgsContract : IContract
	{
		/// <summary>
		/// Type of input data
		/// </summary>
		InputType Type { get; }

		/// <summary>
		/// Data
		/// </summary>
		object Data { get; }
	}
}
