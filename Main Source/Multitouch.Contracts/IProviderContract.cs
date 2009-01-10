using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Windows;
using PipelineHints;

[assembly:SegmentAssemblyName(PipelineSegment.AddInSideAdapter, "Multitouch.AddInSideAdapter")]
[assembly:SegmentAssemblyName(PipelineSegment.AddInView, "Multitouch.AddInView")]
[assembly:SegmentAssemblyName(PipelineSegment.HostSideAdapter, "Multitouch.HostSideAdapter")]
[assembly:SegmentAssemblyName(PipelineSegment.HostView, "Multitouch.HostView")]
[assembly:SegmentAssemblyName(PipelineSegment.Views, "Multitouch.Views")]
[assembly:ShareViews]

namespace Multitouch.Contracts.Contracts
{
	[AddInContract]
	public interface IProviderContract : IContract
	{
		/// <summary>
		/// Starts provider and sending events.
		/// </summary>
		void Start();

		/// <summary>
		/// Stops provider and sending events.
		/// </summary>
		void Stop();

		/// <summary>
		/// Returns <c>True</c> if provider is running, else <c>False</c>.
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Returns <c>True</c> if input provider has configuration
		/// </summary>
		bool HasConfiguration { get; }

		/// <summary>
		/// Returns UIElement representing configuration dialog
		/// </summary>
		UIElement GetConfiguration();

		/// <summary>
		/// Sets kind of image you want to receive.
		/// </summary>
		/// <param name="imageType">Kind of image</param>
		/// <param name="value">if set to <c>true</c> image should be send.</param>
		/// <returns><c>true</c> if image will be send, <c>false</c> if not.</returns>
		bool SendImageType(ImageType imageType, bool value);

		/// <summary>
		/// If <c>true</c> sends every frame, also without any contacts, otherwise only frames with contacts.
		/// </summary>
		bool SendEmptyFrames { get; set; }

		/// <summary>
		/// Raised on any input from device.
		/// </summary>
		/// <param name="handler"></param>
		[EventAdd("NewFrame")]
		void NewFrameAdd(INewFrameEventHandlerContract handler);
		[EventRemove("NewFrame")]
		void NewFrameRemove(INewFrameEventHandlerContract handler);
	}
}
