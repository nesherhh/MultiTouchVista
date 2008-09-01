using System;
using openCV;

namespace OpenCVCommon.Filters
{
	public interface IFilter
	{
		IplImage Apply(IplImage image);
	}
}