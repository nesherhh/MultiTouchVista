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
				public ref class Invert:BaseFilter
				{
				public:
					Invert(void);
					virtual void Invert::ProcessImage(int width, int height, int %bitPerPixel) override;
				};
			}
		}
	}
}