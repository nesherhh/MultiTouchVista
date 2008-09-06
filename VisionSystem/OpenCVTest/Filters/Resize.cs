using System;
using openCV;

namespace OpenCVTest.Filters
{
	class Resize : Filter
	{
		public Resize(FilterContext context)
			: base(context)
		{ }

		protected override IplImage CreateDestination()
		{
			return cvlib.CvCreateImage(cvlib.CvSize(160, 120), (int)cvlib.IPL_DEPTH_8U, 1);
		}

		public override IplImage Apply(IplImage image)
		{
			IplImage result = Destination;
			cvlib.CvResize(ref image, ref result, cvlib.CV_INTER_LINEAR);
			return result;
		}
	}
}