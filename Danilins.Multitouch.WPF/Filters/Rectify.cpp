#include "StdAfx.h"

#include <cv.h>
#include <cxcore.h>

#include "Rectify.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				Rectify::Rectify(void)
				{
				}

				void Rectify::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);
					cvThreshold(source, destination, Level, 255, CV_THRESH_TOZERO);
				}
			}
		}
	}
}