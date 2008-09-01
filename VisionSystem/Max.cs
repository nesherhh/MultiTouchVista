using System;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	public partial class Max : FilterVisualization
	{
		public Max(FilterContext context)
			: base(new MaxFilter(), context)
		{
			InitializeComponent();
			trackBarMin_ValueChanged(trackBarMin, EventArgs.Empty);
		}

		MaxFilter Filter { get { return (MaxFilter)filter; } } 

		void trackBarMin_ValueChanged(object sender, EventArgs e)
		{
			Filter.MaxValue = (byte)trackBarMin.Value;
			labelMin.Text = trackBarMin.Value.ToString();
		}

		class MaxFilter : FilterGrayToGray
		{
			public byte MaxValue { get; set; }

			protected override unsafe void ProcessFilter(BitmapData imageData)
			{
				byte* imgPtr = (byte*)imageData.Scan0;
				for (int y = 0; y < imageData.Height; y++)
				{
					for (int x = 0; x < imageData.Width; x++)
					{
						int offset = y * imageData.Stride + x;

						byte value = imgPtr[offset];
						if(value > MaxValue)
							value = MaxValue;
						imgPtr[offset] = value;
					}
				}
			}
		}
	}
}