#include "StdAfx.h"

#include <cv.h>
#include <cxcore.h>
#include "BaseFilter.h"

using namespace System::Runtime::InteropServices;

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				BaseFilter::BaseFilter(void)
				{
					source = NULL;
					destination = NULL;
				};

				void BaseFilter::ReleaseImage(IplImage* image)
				{
						pin_ptr<IplImage*> pImage = &image;
						cvReleaseImage(pImage);
				};

				array<Byte>^ BaseFilter::Process(int width, int height, int %bitPerPixel, array<Byte>^ sourceImage)
				{
					source = GetImage(width, height, bitPerPixel, sourceImage);

					ProcessImage(width, height,bitPerPixel);
					array<Byte>^ result = GetArray(destination);

					if(source != NULL)
						ReleaseImage(source);
					if(destination != NULL)
						ReleaseImage(destination);

					LastResult = gcnew ResultData(width, height, bitPerPixel, result);
					return result;
				}

				array<Byte>^ BaseFilter::GetArray(IplImage *image)
				{
					array<Byte>^ result = gcnew array<Byte>(image->width * image->height * image->nChannels);
					Marshal::Copy((IntPtr)image->imageData, result, 0, image->imageSize);
					return result;
				}

				IplImage* BaseFilter::GetImage(int width, int height, int bitPerPixel, array<Byte> ^sourceImage)
				{
					int depth = IPL_DEPTH_8U;
					int channel = bitPerPixel / 8;
					IplImage* result = cvCreateImage(cvSize(width, height), depth, channel);
					Marshal::Copy(sourceImage, 0, (IntPtr)result->imageData, sourceImage->Length);
					return result;
				}

				IplImage* BaseFilter::CreateGrayscaleImage(IplImage* source)
				{
					IplImage* result = cvCreateImage(cvSize(source->width, source->height), IPL_DEPTH_8U, 1);
					result->origin = source->origin;  // same vertical flip as source
					return result;
				}
			}
		}
	}
}