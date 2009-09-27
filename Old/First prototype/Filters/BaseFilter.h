#pragma once

#include <cv.h>
#include <cxcore.h>

using namespace System;
using namespace Danilins::Multitouch::Common::Filters;

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				public ref class BaseFilter abstract:IFilter
				{
				public:
					BaseFilter(void);
					
					virtual array<Byte>^ Process(int width, int height, int %bitPerPixel, array<Byte>^ sourceImage);
					virtual property ResultData^ LastResult;

					virtual void ProcessImage(int width, int height, int %bitPerPixel) abstract;
					IplImage* GetImage(int width, int height, int bitPerPixel, array<Byte>^ sourceImage);
					array<Byte>^ GetArray(IplImage* image);
				protected:
					IplImage* source;
					IplImage* destination;
					IplImage* CreateGrayscaleImage(IplImage* source);
					void ReleaseImage(IplImage* image);
				};
			}
		}
	}
}