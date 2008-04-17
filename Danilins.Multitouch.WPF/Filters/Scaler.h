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
				public ref class Scaler:BaseFilter
				{
				public:
					Scaler(void);
					virtual void Scaler::ProcessImage(int width, int height, int %bitPerPixel) override;

					[Range(255)]
					property int Level;
				};
			}
		}
	}
}