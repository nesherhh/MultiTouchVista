using System;

namespace Danilins.Multitouch.Common.Providers
{
	public interface IInputProvider : IDisposable
	{
		Guid Id { get; }
		string Name { get; }
		bool ShowPreview { get; set; }

		ContactInfo[] GetContacts();
		void SetPreviewInputHandler(PreviewPixelHandler handler);
		void SetPreviewOutputHandler(PreviewPixelHandler handler);

		void Start();
		bool IsRunning { get; }
		void Stop();
	}
}