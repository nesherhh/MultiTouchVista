#include "StdAfx.h"

#include <cv.h>
#include <cxcore.h>

#include "SimpleHighpass.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				SimpleHighpass::SimpleHighpass(void)
				{
					buffer = NULL;
					Blur = 17;
					Noise = 3;
				}

				SimpleHighpass::~SimpleHighpass(void)
				{
					if(buffer == NULL)
						ReleaseImage(buffer);
				}

				void SimpleHighpass::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);
					if(buffer == NULL)
						buffer = CreateGrayscaleImage(source);

					int blurParameter = Blur * 2 + 1;
					cvSmooth(source, buffer, CV_BLUR, blurParameter, blurParameter);
					cvSub(source, buffer, buffer);

					int noiseParameter = Noise * 2 + 1;
					cvSmooth(buffer, destination, CV_BLUR, noiseParameter, noiseParameter);
				}
			}
		}
	}
}