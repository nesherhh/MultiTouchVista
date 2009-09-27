using System;
using openCV;
using OpenCVCommon.Filters;

namespace OpenCVTest.Filters
{
	abstract class Filter: IFilter
	{
		private IplImage destination;

		protected FilterContext Context { get; set; }

		protected Filter(FilterContext context)
		{
			Context = context;
		}

		protected virtual IplImage CreateDestination()
		{
			return cvlib.CvCreateImage(cvlib.CvSize((int)Context.Width, (int)Context.Height), (int)cvlib.IPL_DEPTH_8U, 1);
		}

		protected IplImage Destination
		{
			get
			{
				if (destination.ptr == IntPtr.Zero)
					destination = CreateDestination();
				return destination;
			}
		}

		public abstract IplImage Apply(IplImage image);
	}
}
