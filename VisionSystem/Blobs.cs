using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	public partial class Blobs : FilterVisualization
	{
		BlobCounter blobCounter;

		public Blobs(FilterContext context)
			: base(new BlobCounterWrapper(new BlobCounter()), context)
		{
			InitializeComponent();
			blobCounter = ((BlobCounterWrapper)filter).BlobCounter;
			//blobCounter.MinHeight = blobCounter.MinWidth = 10;
			//blobCounter.MaxHeight = blobCounter.MaxWidth = 50;
			blobCounter.FilterBlobs = true;
		}

		protected override void FilterApplied(Bitmap image)
		{
			Bitmap b = new Bitmap(image);
			ProcessBlobs(blobCounter.GetObjectInformation(), b);
			base.FilterApplied(b);
		}

		void ProcessBlobs(Blob[] blobs, Bitmap image)
		{
			using (Graphics g = Graphics.FromImage(image))
			{
				foreach (Blob blob in blobs)
					g.FillEllipse(Brushes.Red, blob.Rectangle);
			}
		}

		class BlobCounterWrapper : IFilter
		{
			public BlobCounter BlobCounter { get; set; }

			public BlobCounterWrapper(BlobCounter blobCounter)
			{
				BlobCounter = blobCounter;
			}

			public Bitmap Apply(Bitmap image)
			{
				BlobCounter.ProcessImage(image);
				return image;
			}

			public Bitmap Apply(BitmapData imageData)
			{
				BlobCounter.ProcessImage(imageData);
				return new Bitmap(imageData.Width, imageData.Height, imageData.Stride, imageData.PixelFormat, imageData.Scan0);
			}
		}
	}
}