using System;
using openCV;

namespace OpenCVTest.Filters
{
	class Resize : Filter
	{
		IplImage result;

		public Resize(FilterContext context)
			: base(context)
		{
			result = cvlib.CvCreateImage(cvlib.CvSize(160, 120), (int)cvlib.IPL_DEPTH_8U, 1);
		}

		public override IplImage Apply(IplImage image)
		{
			cvlib.CvResize(ref image, ref result, cvlib.CV_INTER_LINEAR);
			return result;
		}
	}
}