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
				public ref class Rectify:BaseFilter
				{
				public:
					Rectify(void);
					virtual void Rectify::ProcessImage(int width, int height, int %bitPerPixel) override;

					[Range(255)]
					property int Level;
				};
			}
		}
	}
}