using System;
using openCV;

namespace OpenCVTest.Filters
{
	class Grayscale : Filter
	{
		public Grayscale(FilterContext context)
			: base(context)
		{
		}

		public override IplImage Apply(IplImage image)
		{
			IplImage destination = Destination;
			cvlib.CvCvtColor(ref image, ref destination, cvlib.CV_BGR2GRAY);
			return destination;
		}
	}
}