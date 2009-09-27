using System;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	public partial class Min : FilterVisualization
	{
		public Min(FilterContext context)
			: base(new MinFilter(), context)
		{
			InitializeComponent();
			trackBarMin_ValueChanged(trackBarMin, EventArgs.Empty);
		}

		MinFilter Filter { get { return (MinFilter)filter; } } 

		void trackBarMin_ValueChanged(object sender, EventArgs e)
		{
			Filter.MinValue = (byte)trackBarMin.Value;
			labelMin.Text = trackBarMin.Value.ToString();
		}

		class MinFilter : FilterGrayToGray
		{
			public byte MinValue { get; set; }

			protected override unsafe void ProcessFilter(BitmapData imageData)
			{
				byte* imgPtr = (byte*)imageData.Scan0;
				for (int y = 0; y < imageData.Height; y++)
				{
					for (int x = 0; x < imageData.Width; x++)
					{
						int offset = y * imageData.Stride + x;

						byte value = imgPtr[offset];
						if(value < MinValue)
							value = MinValue;
						imgPtr[offset] = value;
					}
				}
			}
		}
	}
}