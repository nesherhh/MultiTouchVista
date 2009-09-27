using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using openCV;
using OpenCVCommon;
using OpenCVFilterLib;

namespace OpenCVTest.Filters
{
	public partial class BlobsVisualization : FilterVisualization
	{
		private IplImage previewImage;
		private GrayscaleY grayscaleFilter;

		BlobsFilter Filter { get { return (BlobsFilter)filter; } }

		public BlobsVisualization(FilterContext context)
			: base(new BlobsFilter(), context)
		{
			InitializeComponent();
			grayscaleFilter = new GrayscaleY();

			Filter.MinBlob = 10;
			Filter.MaxBlob = 500;
		}

		protected override void FilterApplied(IplImage image)
		{
			IList<BlobEllipse> blobs = Filter.Blobs;
			synchronizationContext.Send(state =>
			{
				labelBlobsCount.Text = blobs.Count.ToString();
			}, null);

			ShowPreview(image);
		}

		void ShowPreview(IplImage image)
		{
			if (previewImage.ptr == IntPtr.Zero)
				previewImage = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_8U, 3);

			cvlib.CvCvtColor(ref image, ref previewImage, cvlib.CV_GRAY2BGR);

			Bitmap bitmap = cvlib.ToBitmap(previewImage, false);
            
			Bitmap grayscale = grayscaleFilter.Apply(bitmap);
            ImageStatistics imageStatistics = new ImageStatistics(grayscale);
			Histogram grayHistogram = imageStatistics.GrayWithoutBlack;


			PaintBlobs(bitmap);

			synchronizationContext.Send(state =>
			                            {
			                            	UpdatePictureBox(bitmap);
			                            	histogram.Values = imageStatistics.Gray.Values;
			                            	propertyGrid1.SelectedObject = grayHistogram;
			                            }, null);
			bitmap.Dispose();
		}

		void PaintBlobs(Bitmap bitmap)
		{
			using(Graphics g = Graphics.FromImage(bitmap))
			{
				foreach (BlobEllipse blob in Filter.Blobs)
				{
					float x = blob.Center.X;
					float y = blob.Center.Y;
					float width = blob.Size.Width;
					float height = blob.Size.Height;

					Matrix matrix = g.Transform;
					matrix.Translate(-x, -y, MatrixOrder.Append);
					matrix.Rotate(blob.Angle, MatrixOrder.Append);
					matrix.Translate(x, y, MatrixOrder.Append);
					g.Transform = matrix;

					float drawX = x - width / 2;
					float drawY = y - height / 2;
					g.FillEllipse(Brushes.Red, drawX, drawY, width, height);
					g.DrawString(blob.Id.ToString(), SystemFonts.DefaultFont, Brushes.White, drawX, drawY);

					matrix.Translate(-x, -y, MatrixOrder.Append);
					matrix.Rotate(-blob.Angle, MatrixOrder.Append);
					matrix.Translate(x, y, MatrixOrder.Append);
					g.Transform = matrix;
				}
			}
		}
	}
}