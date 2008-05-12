#pragma once

#include "BaseFilter.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				public ref class SimpleHighpass:BaseFilter
				{
				public:
					SimpleHighpass(void);
					~SimpleHighpass(void);
					virtual void SimpleHighpass::ProcessImage(int width, int height, int %bitPerPixel) override;

					[Range(255)]
					property int Blur;
					[Range(255)]
					property int Noise;
				private:
					IplImage* buffer;
				};
			}
		}
	}
}