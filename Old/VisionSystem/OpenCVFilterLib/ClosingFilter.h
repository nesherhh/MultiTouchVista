#pragma once

#include "cv.h"

using namespace System;
using namespace OpenCVCommon::Filters;

namespace OpenCVFilterLib
{
	public ref class ClosingFilter : IFilter
	{
	public:

		ClosingFilter(void)
		{
		}

		virtual openCV::IplImage Apply(openCV::IplImage frame)
		{
			IntPtr ptr = frame.ptr;
			void* p = ptr.ToPointer();
			IplImage* img = (IplImage*)p;

			cvDilate(img, img);
			cvErode(img, img);

			return frame;
		}
	};
}