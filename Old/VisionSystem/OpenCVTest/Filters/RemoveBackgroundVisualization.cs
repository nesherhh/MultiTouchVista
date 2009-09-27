using System;

namespace OpenCVTest.Filters
{
	public partial class RemoveBackgroundVisualization : FilterVisualization
	{
		public RemoveBackgroundVisualization(FilterContext context)
			: base(new RemoveBackground(context), context)
		{
			InitializeComponent();
			trackBarTreshold.Value = Filter.Threshold;
			trackBarTreshold_ValueChanged(trackBarTreshold, EventArgs.Empty);
		}

		RemoveBackground Filter { get { return (RemoveBackground)filter; } }

		private void trackBarTreshold_ValueChanged(object sender, EventArgs e)
		{
			Filter.Threshold = trackBarTreshold.Value;
			labelThreshold.Text = trackBarTreshold.Value.ToString();
		}
	}
}