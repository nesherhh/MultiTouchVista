#pragma once

#include "cv.h"
#include "blobresult.h"

using namespace System;
using namespace OpenCVCommon::Filters;

namespace OpenCVFilterLib
{

	public ref class BlobsFilter : IFilter
	{
	public:

		BlobsFilter(void)
		{
			cvNamedWindow("Blobs");
		}

		virtual openCV::IplImage Apply(openCV::IplImage frame)
		{
			IntPtr ptr = frame.ptr;
			void* p = ptr.ToPointer();
			IplImage* img = (IplImage*)p;

			CBlobResult blobs;

			blobs = CBlobResult(img, NULL, 100, true);

			IplImage *outputImage = cvCreateImage(cvGetSize(img), IPL_DEPTH_8U, 3);
			cvMerge(img, img, img, NULL, outputImage);

			blobs.Filter(blobs, B_EXCLUDE, CBlobGetArea(), B_GREATER, 5000);
			blobs.Filter(blobs, B_EXCLUDE, CBlobGetArea(), B_LESS, 10);

			int blobsCount = blobs.GetNumBlobs();
			for(int i = 0; i < blobsCount; i++)
			{
				CBlob blob = blobs.GetBlob(i);
				int label = blob.Label();
				blob.FillBlob(outputImage, CV_RGB(255,0,0));
			}

			cvShowImage("Blobs", outputImage);

			return frame;
		}
	};
}