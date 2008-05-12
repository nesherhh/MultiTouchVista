using System;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Providers;

namespace Danilins.Multitouch.Providers
{
	public abstract class InputProvider : IInputProvider
	{
		protected PreviewPixelHandler inputPreviewHandler;
		protected PreviewPixelHandler outputPreviewHandler;
		protected bool isRunning;

		public abstract void Dispose();
		public abstract Guid Id { get; }
		public abstract string Name { get; }

		public bool ShowPreview { get; set; }

		public ContactInfo[] GetContacts()
		{
			return GetContactsCore();
		}

		protected abstract ContactInfo[] GetContactsCore();

		public void SetPreviewInputHandler(PreviewPixelHandler handler)
		{
			inputPreviewHandler = handler;
		}

		public void SetPreviewOutputHandler(PreviewPixelHandler handler)
		{
			outputPreviewHandler = handler;
		}

		public abstract void Start();

		public bool IsRunning
		{
			get { return isRunning; }
		}

		public abstract void Stop();
	}
}