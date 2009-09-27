using System;
using System.Collections.ObjectModel;
using openCV;
using OpenCVCommon.Filters;

namespace OpenCVTest
{
	class FilterPipline : Collection<IFilter>, IFilter
	{
		public IplImage Apply(IplImage image)
		{
			IplImage result = image;
			if (Count > 0)
			{
				foreach (IFilter filter in this)
					result = filter.Apply(result);
			}
			return result;
		}
	}
}