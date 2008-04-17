#include "StdAfx.h"

#include <cv.h>
#include <cxcore.h>

#include "RemoveBackground.h"

namespace Danilins
{
	namespace Multitouch
	{
		namespace Framework
		{
			namespace Filters
			{
				RemoveBackground::RemoveBackground(void)
				{
					reference = NULL;
					mask = NULL;
					recapture = true;
				}

				RemoveBackground::~RemoveBackground(void)
				{
					if(reference != NULL)
						ReleaseImage(reference);
					if(mask != NULL)
						ReleaseImage(mask);
				}

				void RemoveBackground::ProcessImage(int width, int height, int %bitPerPixel)
				{
					destination = CreateGrayscaleImage(source);

					if(recapture)
					{
						if(reference)
							cvCopy(source, reference);
						else
							reference = cvCloneImage(source);
						
						if(!mask)
						{
							mask = cvCreateImage(cvGetSize(reference), reference->depth, 1);
							mask->origin = reference->origin;
							cvSet(mask,cvScalar(0,0,0));
						}
						cvAdd(reference, mask, reference);
						recapture = false;
					}
					cvSub(source, reference, destination);
				}
			}
		}
	}
}