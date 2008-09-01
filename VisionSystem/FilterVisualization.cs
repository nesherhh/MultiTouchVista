using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;

namespace WindowsFormsApplication1
{
	public partial class FilterVisualization : UserControl, IInPlaceFilter, IFilter
	{
		protected readonly IFilter filter;
		protected readonly FilterContext context;
		protected readonly SynchronizationContext synchronizationContext;

		public FilterVisualization()
		{
			InitializeComponent();
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

		public void ApplyInPlace(Bitmap image)
		{
			BeforeApplyingFilter(image);
			IInPlaceFilter inPlaceFilter = filter as IInPlaceFilter;
			if(inPlaceFilter != null)
				inPlaceFilter.ApplyInPlace(image);
			FilterApplied(image);
		}

		public void ApplyInPlace(BitmapData imageData)
		{
			BeforeApplyingFilter(imageData);
			IInPlaceFilter inPlaceFilter = filter as IInPlaceFilter;
			if (inPlaceFilter != null)
				inPlaceFilter.ApplyInPlace(imageData);
			FilterApplied(imageData);
		}

		public Bitmap Apply(Bitmap image)
		{
			BeforeApplyingFilter(image);
			Bitmap filteredImage = filter.Apply(image);
			FilterApplied(filteredImage);
			return filteredImage;
		}

		public Bitmap Apply(BitmapData imageData)
		{
			BeforeApplyingFilter(imageData);
			Bitmap filteredImage = filter.Apply(imageData);
			FilterApplied(filteredImage);
			return filteredImage;
		}

		protected virtual void BeforeApplyingFilter(BitmapData imageData)
		{ }

		protected virtual void BeforeApplyingFilter(Bitmap image)
		{ }

		protected virtual void FilterApplied(Bitmap image)
		{
			try
			{
				ImageStatistics imageStatistics = new ImageStatistics(image);
				Histogram grayHistogram = imageStatistics.GrayWithoutBlack;

				synchronizationContext.Send(state =>
				                            {
												UpdatePictureBox(image);
												if(imageStatistics.IsGrayscale)
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

		void FilterApplied(BitmapData imageData)
		{
			FilterApplied(new Bitmap(imageData.Width, imageData.Height, imageData.Stride, imageData.PixelFormat, imageData.Scan0));
		}
	}
}