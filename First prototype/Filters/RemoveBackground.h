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
				public ref class RemoveBackground : BaseFilter
				{
				public:
					RemoveBackground(void);
					~RemoveBackground(void);
					virtual void RemoveBackground::ProcessImage(int width, int height, int %bitPerPixel) override;
				private:
					IplImage* reference;
					IplImage* mask;
					bool recapture;
				};
			}
		}
	}
}