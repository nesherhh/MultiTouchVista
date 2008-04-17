#include "StdAfx.h"

#include <cv.h>
#include <cxcore.h>

#include "Scaler.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				Scaler::Scaler(void)
				{
					Level = 70;
				}

				void Scaler::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);
					cvMul(source, source, destination, (float)Level / 128.0f);
				}
			}
		}
	}
}