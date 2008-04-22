using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using PipelineHints;

[assembly:SegmentAssemblyName(PipelineSegment.AddInSideAdapter, "Multitouch.AddInSideAdapter")]
[assembly:SegmentAssemblyName(PipelineSegment.AddInView, "Multitouch.AddInView")]
[assembly:SegmentAssemblyName(PipelineSegment.HostSideAdapter, "Multitouch.HostSideAdapter")]
[assembly:SegmentAssemblyName(PipelineSegment.HostView, "Multitouch.HostView")]

namespace Multitouch.Contracts.Contracts
{
	[AddInContract]
	public interface IProviderContract : IContract
	{
		[Comment(
		"/// <summary>"+
		"/// Starts provider and sending events."+
		"/// </summary>"
		)]
		void Start();
		[Comment(
		"/// <summary>"+
		"/// Stops provider and sending events."+
		"/// </summary>"
		)]
		void Stop();
		[Comment(
		"/// <summary>"+
		"/// Returns <c>True</c> if provider is running, else <c>False</c>."+
		"/// </summary>"
		)]
		bool IsRunning { get; }

		[Comment(
		"/// <summary>"+
		"/// Raised when contact changes."+
		"/// </summary>"
		)]
		[EventAdd("ContactChanged")]
		void ContactChangedAdd(IContactChangedEventHandlerContract handler);
		[EventRemove("ContactChanged")]
		void ContactChangedRemove(IContactChangedEventHandlerContract handler);
	}
}
