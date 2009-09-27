using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;
using openCV;
using IFilter=OpenCVCommon.Filters.IFilter;

namespace OpenCVTest
{
	public partial class FilterVisualization : UserControl, IFilter
	{
		protected readonly IFilter filter;
		protected readonly FilterContext context;
		protected readonly SynchronizationContext synchronizationContext;
		private IplImage previewImage;
		private GrayscaleY grayscaleFilter;

		public FilterVisualization()
		{
			InitializeComponent();
			grayscaleFilter = new GrayscaleY();
			synchronizationContext = SynchronizationContext.Current;
			if(synchronizationContext == null)
				synchronizationContext = new SynchronizationContext();
		}

		public FilterVisualization(IFilter filter, FilterContext context)
			: this()
		{
			this.filter = filter;
			this.context = context;
			groupBox.Text = filter.ToString();
		}

		public IplImage Apply(IplImage image)
		{
			BeforeApplyingFilter(image);
			IplImage filteredImage = filter.Apply(image);
			FilterApplied(filteredImage);
			return filteredImage;
		}

		protected virtual void BeforeApplyingFilter(IplImage image)
		{ }

		protected virtual void FilterApplied(IplImage image)
		{
			try
			{
				if (previewImage.ptr == IntPtr.Zero)
					previewImage = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_8U, 3);

				cvlib.CvCvtColor(ref image, ref previewImage, cvlib.CV_GRAY2BGR);

				Bitmap bitmap = cvlib.ToBitmap(previewImage, false);
                Bitmap grayscale = grayscaleFilter.Apply(bitmap);
				bitmap.Dispose();


				ImageStatistics imageStatistics = new ImageStatistics(grayscale);
				Histogram grayHistogram = imageStatistics.GrayWithoutBlack;

				synchronizationContext.Send(state =>
				                            {
												UpdatePictureBox(grayscale);
				                            	histogram.Values = imageStatistics.Gray.Values;
				                            	propertyGrid1.SelectedObject = grayHistogram;
				                            }, null);
			}
			catch (Exception e)
			{
				Trace.TraceInformation(e.Message);
			}
		}

		protected void UpdatePictureBox(Bitmap image)
		{
			using (Graphics graphics = Graphics.FromHwnd(panelPreview.Handle))
			{
				graphics.DrawImage(image, 0, 0, panelPreview.Width, panelPreview.Height);
			}
		}
	}
}