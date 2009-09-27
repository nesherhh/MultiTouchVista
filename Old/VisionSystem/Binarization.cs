using System;
using System.Drawing;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math;

namespace WindowsFormsApplication1
{
	partial class Binarization : FilterVisualization
	{
		ImageStatistics statistics;
		bool autoThreshold;

		public Binarization(FilterContext context)
			: base(new Threshold(), context)
		{
			InitializeComponent();
			trackBarThreshold_ValueChanged(trackBarThreshold, EventArgs.Empty);
			checkBoxAutoThreshold_CheckedChanged(checkBoxAutoThreshold, EventArgs.Empty);
		}

		Threshold Filter { get { return (Threshold)filter; } }

		protected override void BeforeApplyingFilter(Bitmap image)
		{
			statistics = new ImageStatistics(image);
			Histogram hist = statistics.GrayWithoutBlack;

			byte value = (byte)(hist.Max - hist.Mean);

			if (autoThreshold)
			{

				Filter.ThresholdValue = value;

				synchronizationContext.Send(state =>
				                            {
				                            	trackBarThreshold.Maximum = hist.Max;
				                            	trackBarThreshold.Value = value;
				                            }, null);
			}
			base.BeforeApplyingFilter(image);
		}

		private void trackBarThreshold_ValueChanged(object sender, EventArgs e)
		{
			labelThreshold.Text = trackBarThreshold.Value.ToString();
			Filter.ThresholdValue = (byte)trackBarThreshold.Value;
		}

		private void checkBoxAutoThreshold_CheckedChanged(object sender, EventArgs e)
		{
			autoThreshold = checkBoxAutoThreshold.Checked;
		}
	}
}