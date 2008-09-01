using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	partial class RemoveBackground : FilterVisualization
	{
		RemoveBackgroundFilter Filter { get { return (RemoveBackgroundFilter)filter; } }

		public RemoveBackground(FilterContext context)
			: base(new RemoveBackgroundFilter(context), context)
		{
			InitializeComponent();
			trackBarThreshold_ValueChanged(trackBarThreshold, EventArgs.Empty);
		}

		protected override void BeforeApplyingFilter(Bitmap image)
		{
			if (context.Background == null)
				context.Background = new GrayscaleY().Apply(image);

			base.BeforeApplyingFilter(image);
		}

		private void trackBarThreshold_ValueChanged(object sender, EventArgs e)
		{
			Filter.Threshold = trackBarThreshold.Value;
			labelThreshold.Text = trackBarThreshold.Value.ToString();
		}

		internal class RemoveBackgroundFilter : FilterGrayToGray
		{
			readonly FilterContext context;

			public int Threshold { get; set; }
			Bitmap mask;
			Subtract subtract;

			public RemoveBackgroundFilter(FilterContext context)
			{
				this.context = context;
				Threshold = 8;
				subtract = new Subtract();
			}

			protected override void ProcessFilter(BitmapData imageData)
			{
				if (mask == null)
					mask = new Bitmap(context.Background.Width, context.Background.Height, context.Background.PixelFormat);

				unsafe
				{
					int width = imageData.Width;
					int height = imageData.Height;
					PixelFormat pixelFormat = imageData.PixelFormat;

					BitmapData imageBits = imageData;
					BitmapData backBits = context.Background.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, pixelFormat);
					BitmapData maskBits = mask.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pixelFormat);

					int stride = imageBits.Stride;

					byte* imagePtr = (byte*)imageBits.Scan0;
					byte* backPtr = (byte*)backBits.Scan0;
					byte* maskPtr = (byte*)maskBits.Scan0;

					for (int y = 0; y < height; y++)
					{
						for (int x = 0; x < width; x++)
						{
							int offset = y * stride + x;

							byte imagePixel = imagePtr[offset];
							byte backPixel = backPtr[offset];

							byte maskPixel = 255;
							if (imagePixel - backPixel> Threshold)
								maskPixel = 0;

							maskPtr[offset] = maskPixel;
						}
					}

					context.Background.UnlockBits(backBits);
					mask.UnlockBits(maskBits);
				}

				subtract.OverlayImage = mask;
				subtract.ApplyInPlace(imageData);
			}
		}
	}
}