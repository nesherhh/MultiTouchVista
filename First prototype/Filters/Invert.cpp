#include "StdAfx.h"

#include "Invert.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				Invert::Invert(void)
				{
				}

				void Invert::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);
					cvNot(source, destination);
				}
			}
		}
	}
}