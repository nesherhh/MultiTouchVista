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
				public ref class Grayscale:BaseFilter
				{
				public:
					Grayscale(void);
					virtual void Grayscale::ProcessImage(int width, int height, int %bitPerPixel) override;
				};
			}
		}
	}
}