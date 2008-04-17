#include "StdAfx.h"
#include "Grayscale.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				Grayscale::Grayscale(void)
				{
				}

				void Grayscale::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);

					// Note: the destination must have one channel.
					if(source->nChannels != 1 && destination->nChannels == 1) 
					{
						if(_strcmpi(source->colorModel, "BGRA") == 0)
							cvCvtColor(source, destination, CV_BGRA2GRAY);
						else if(_strcmpi(source->colorModel, "BGR") == 0)
							cvCvtColor(source, destination, CV_BGR2GRAY);
						else if(_strcmpi(source->colorModel, "RGB") == 0) 
							cvCvtColor(source, destination, CV_RGB2GRAY);						
					}

					bitPerPixel = 8;
				}
			}
		}
	}
}