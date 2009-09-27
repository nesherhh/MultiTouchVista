#pragma once

#include "cv.h"
#include "blobresult.h"

using namespace System;
using namespace System::Drawing;
using namespace System::Collections::Generic;
using namespace OpenCVCommon;
using namespace OpenCVCommon::Filters;

namespace OpenCVFilterLib
{

	public ref class BlobsFilter : IFilter
	{
	private:
		IList<BlobEllipse>^ blobEllipses;

	public:

		BlobsFilter(void)
		{
			blobEllipses = gcnew List<BlobEllipse>();
		}

		virtual openCV::IplImage Apply(openCV::IplImage frame)
		{
			blobEllipses->Clear();

			IntPtr ptr = frame.ptr;
			void* p = ptr.ToPointer();
			IplImage* img = (IplImage*)p;

			CBlobResult blobs;

			blobs = CBlobResult(img, NULL, 100, true);

			blobs.Filter(blobs, B_EXCLUDE, CBlobGetArea(), B_GREATER, MaxBlob);
			blobs.Filter(blobs, B_EXCLUDE, CBlobGetArea(), B_LESS, MinBlob);

			int blobsCount = blobs.GetNumBlobs();
			for(int i = 0; i < blobsCount; i++)
			{
				CBlob blob = blobs.GetBlob(i);
				CvBox2D box = blob.GetEllipse();

				Point^ center = gcnew Point(box.center.x, box.center.y);
				Size^ size = gcnew Size(box.size.height, box.size.width);
				BlobEllipse^ blobEllipse = gcnew BlobEllipse(*center, *size, box.angle, blob.Label());
				blobEllipses->Add(*blobEllipse);
			}
			return frame;
		}

		property double MinBlob;
		property double MaxBlob;

		property IList<BlobEllipse>^ Blobs
		{
			IList<BlobEllipse>^ get()
			{
				return blobEllipses;
			}
		}
	};
}