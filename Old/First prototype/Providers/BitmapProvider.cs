using System;
using System.Threading;
using System.Windows;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.Providers.Configuration.Models;
using Danilins.Multitouch.Providers.Configuration.View;
using Danilins.Multitouch.Providers.Filters;

namespace Danilins.Multitouch.Providers
{
	public abstract class BitmapProvider:InputProvider, IConfigurableInputProvider
	{
		private BlobCollector collector;
		private ContactRecognizer recognizer;
		private FilterPipe filters;
		protected AutoResetEvent nextFrame;
		protected string sectionName;

		public event EventHandler FilterUpdated;

		protected BitmapProvider()
		{
			nextFrame = new AutoResetEvent(false);
			collector = new BlobCollector();
			recognizer = new ContactRecognizer();
			filters = new FilterPipe();
			sectionName = string.Format("filtersSection-{0}", Id);
		}

		protected override ContactInfo[] GetContactsCore()
		{
			WaitForNextFrame();
			int width;
			int height;
			int bitPerPixel;
			byte[] inputBitmap = GetData(out width, out height, out bitPerPixel);
			if(inputPreviewHandler != null && ShowPreview)
				inputPreviewHandler(width, height, bitPerPixel, inputBitmap);

			byte[] processedBitmap = filters.Process(width, height, ref bitPerPixel, inputBitmap);
			if(outputPreviewHandler != null && ShowPreview)
				outputPreviewHandler(width, height, bitPerPixel, processedBitmap);

			if (FilterUpdated != null)
				FilterUpdated(this, EventArgs.Empty);

			collector.Process(width, height, processedBitmap);
			return recognizer.Recognize(collector.GetBlobs());
		}

		private void WaitForNextFrame()
		{
			nextFrame.WaitOne();
		}

		public FilterPipe Filters
		{
			get { return filters; }
		}

		protected abstract byte[] GetData(out int width, out int height, out int bitPerPixel);

		public FrameworkElement GetConfigurationUI()
		{
			FilterConfigurationView view = new FilterConfigurationView();
			view.Model = new FilterConfigurationModel(this);
			return view;
		}

		public abstract void Save();
	}
}
