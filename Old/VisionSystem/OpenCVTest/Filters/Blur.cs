using System;
using openCV;

namespace OpenCVTest.Filters
{
	class Blur : Filter
	{
		public Blur(FilterContext context)
			: base(context)
		{
		}

		public override IplImage Apply(IplImage image)
		{
			cvlib.CvSmooth(ref image, ref image, cvlib.CV_GAUSSIAN, 5, 5, 0, 0);
			return image;
		}
	}
}