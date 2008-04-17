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
				public ref class Threshold:BaseFilter
				{
				public:
					Threshold(void);
					virtual void Threshold::ProcessImage(int width, int height, int %bitPerPixel) override;
					
					[Range(255)]
					property int ThresholdValue;
				private:
					float threshold;
					float overallMax;
					int reinitDynamicStatisticsFrames;
				};
			}
		}
	}
}