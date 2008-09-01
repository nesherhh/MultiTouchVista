using System;
using openCV;

namespace OpenCVTest.Filters
{
	class RemoveBackground : Filter
	{
		IplImage mask;

		public RemoveBackground(FilterContext context)
			: base(context)
		{
			Threshold = 17;
		}

		public int Threshold { get; set; }

		public override IplImage Apply(IplImage image)
		{
			if (Context.Background.ptr == IntPtr.Zero)
			{
				Context.Background = cvlib.CvCloneImage(ref image);
				mask = cvlib.CvCreateImage(cvlib.CvGetSize(ref image), (int)cvlib.IPL_DEPTH_8U, 1);
			}

			unsafe
			{
				byte* imagePtr = (byte*)image.imageData;
				byte* backgroundPtr = (byte*)Context.Background.imageData;
				byte* maskPtr = (byte*)mask.imageData;

				for (int y = 0; y < image.height; y++)
				{
					for (int x = 0; x < image.width; x++)
					{
						int address = y * image.widthStep + x;
						byte imgByte = imagePtr[address];
						byte backByte = backgroundPtr[address];
						
						byte value = 0;
						if (imgByte - backByte > Threshold)
							value = 255;

						if (value == 0)
							imagePtr[address] = 0;
						else
							imagePtr[address] = (byte)(imagePtr[address] - backgroundPtr[address]);
					}
				}
			}

			return image;
		}
	}
}