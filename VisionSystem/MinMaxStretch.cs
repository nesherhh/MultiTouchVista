using System;
using System.Drawing.Imaging;
using AForge;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	public partial class MinMaxStretch : FilterVisualization
	{
		MinMaxLinearStretch Filter { get { return (MinMaxLinearStretch)filter; } }

		public MinMaxStretch(FilterContext context)
			: base(new MinMaxLinearStretch(), context)
		{
			InitializeComponent();
			trackBarMax_ValueChanged(trackBarMax, EventArgs.Empty);
			trackBarMin_ValueChanged(trackBarMin, EventArgs.Empty);
		}

		void trackBarMax_ValueChanged(object sender, EventArgs e)
		{
			Filter.Max = trackBarMax.Value;
			labelMax.Text = trackBarMax.Value.ToString();
		}

		void trackBarMin_ValueChanged(object sender, EventArgs e)
		{
			Filter.Min = trackBarMin.Value;
			labelMin.Text = trackBarMin.Value.ToString();
		}

		class MinMaxLinearStretch : FilterGrayToGray
		{
			LevelsLinear levelsLinear;
			public int Min { get; set; }
			public int Max { get; set; }

			public MinMaxLinearStretch()
			{
				levelsLinear = new LevelsLinear();
			}

			protected override void ProcessFilter(BitmapData imageData)
			{
				levelsLinear.InGray = new IntRange(Min, Max);
				levelsLinear.ApplyInPlace(imageData);
			}
		}
	}
}