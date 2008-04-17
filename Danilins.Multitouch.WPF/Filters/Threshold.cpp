#include "StdAfx.h"

#include <cv.h>
#include <cxcore.h>

#include "Threshold.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				Threshold::Threshold(void)
				{
					reinitDynamicStatisticsFrames = 6;
					overallMax = 0.0f;
					threshold = 0.0f;
				}

				void Threshold::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);

					float localMax = 0.0f;
					float localAverage = 0.0f;

					uchar *src = (uchar*) source->imageData;
					uchar *srcEnd = src + source->imageSize;
					uchar *dest = (uchar*) destination->imageData;

					while (src < srcEnd) {
						uchar pixel = *(src++);
						
						// did we reach a new local max?
						float pixelFloat = pixel / 255.0f;
						if (pixelFloat > localMax)
							localMax = pixelFloat;
						localAverage += pixelFloat;

						// drop pixels below threshold
						if (pixelFloat < threshold)
							*(dest++) = 0;
						else
							*(dest++) = 255;
					}

					localAverage /= source->width * source->height;

					if (reinitDynamicStatisticsFrames > 0) {
						reinitDynamicStatisticsFrames--;
						if (reinitDynamicStatisticsFrames == 0) {
							// reset
							threshold = localMax * 1.8f;
							overallMax = 2 * threshold - localAverage;
						} else {
							threshold = 1.0f;
						}
					} else {
						if (localMax > overallMax) {
							overallMax = localMax;
						} else if (localMax > threshold) {
							overallMax = overallMax * 0.995f + localMax * 0.005f;
						}
				
						threshold = (localAverage + overallMax) * 0.5f;
					}
				}
			}
		}
	}
}