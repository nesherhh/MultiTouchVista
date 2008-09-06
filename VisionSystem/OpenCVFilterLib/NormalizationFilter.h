#pragma once

#include "cv.h"
#include "highgui.h"

using namespace System;
using namespace OpenCVCommon::Filters;

namespace OpenCVFilterLib
{
	public ref class NormalizationFilter : IFilter
	{
	public:

		NormalizationFilter(void)
		{
			//cvNamedWindow("Normalization", 1);
		}

		virtual openCV::IplImage Apply(openCV::IplImage frame)
		{
			IntPtr ptr = frame.ptr;
			void* p = ptr.ToPointer();
			IplImage* img = (IplImage*)p;

			cvEqualizeHist(img, img);

			return frame;
		}

		void cvEqualizeHist( const CvArr* src, CvArr* dst )
		{
			CvHistogram* hist = 0;
			CvMat* lut = 0;
		    
			CV_FUNCNAME( "cvEqualizeHist" );

			__BEGIN__;

			int i, hist_sz = 256;
			CvSize img_sz;
			float scale;
			float* h;
			int sum = 0;
			int type;
		    
			CV_CALL( type = cvGetElemType( src ));
			if( type != CV_8UC1 )
				CV_ERROR( CV_StsUnsupportedFormat, "Only 8uC1 images are supported" );

			CV_CALL( hist = cvCreateHist( 1, &hist_sz, CV_HIST_ARRAY ));
			CV_CALL( lut = cvCreateMat( 1, 256, CV_8UC1 ));
			CV_CALL( cvCalcArrHist( (CvArr**)&src, hist ));
			CV_CALL( img_sz = cvGetSize( src ));
			scale = 255.f/(img_sz.width*img_sz.height);
			//h = (float*)hist->bins;
			h = (float*)cvPtr1D( hist->bins, 0, 0);

			for( i = 0; i < hist_sz; i++ )
			{
				sum += cvRound(h[i]);
				lut->data.ptr[i] = (uchar)cvRound(sum*scale);
			}

			lut->data.ptr[0] = 0;
			CV_CALL( cvLUT( src, dst, lut ));

			__END__;

			cvReleaseHist(&hist);
			cvReleaseMat(&lut);
		}

		// Returns pointer to specified element of array (linear index is used)
		uchar* cvPtr1D( const CvArr* arr, int idx, int* _type )
		{
			uchar* ptr = 0;
		    
			CV_FUNCNAME( "cvPtr1D" );

			__BEGIN__;

			if( CV_IS_MAT( arr ))
			{
				CvMat* mat = (CvMat*)arr;

				int type = CV_MAT_TYPE(mat->type);
				int pix_size = CV_ELEM_SIZE(type);

				if( _type )
					*_type = type;
		        
				// the first part is mul-free sufficient check
				// that the index is within the matrix
				if( (unsigned)idx >= (unsigned)(mat->rows + mat->cols - 1) &&
					(unsigned)idx >= (unsigned)(mat->rows*mat->cols))
					CV_ERROR( CV_StsOutOfRange, "index is out of range" );

				if( CV_IS_MAT_CONT(mat->type))
				{
					ptr = mat->data.ptr + (size_t)idx*pix_size;
				}
				else
				{
					int row, col;
					if( mat->cols == 1 )
						row = idx, col = 0;
					else
						row = idx/mat->cols, col = idx - row*mat->cols;
					ptr = mat->data.ptr + (size_t)row*mat->step + col*pix_size;
				}
			}
			else if( CV_IS_IMAGE_HDR( arr ))
			{
				IplImage* img = (IplImage*)arr;
				int width = !img->roi ? img->width : img->roi->width;
				int y = idx/width, x = idx - y*width;

				ptr = cvPtr2D( arr, y, x, _type );
			}
			else if( CV_IS_MATND( arr ))
			{
				CvMatND* mat = (CvMatND*)arr;
				int j, type = CV_MAT_TYPE(mat->type);
				size_t size = mat->dim[0].size;

				if( _type )
					*_type = type;

				for( j = 1; j < mat->dims; j++ )
					size *= mat->dim[j].size;

				if((unsigned)idx >= (unsigned)size )
					CV_ERROR( CV_StsOutOfRange, "index is out of range" );

				if( CV_IS_MAT_CONT(mat->type))
				{
					int pix_size = CV_ELEM_SIZE(type);
					ptr = mat->data.ptr + (size_t)idx*pix_size;
				}
				else
				{
					ptr = mat->data.ptr;
					for( j = mat->dims - 1; j >= 0; j-- )
					{
						int sz = mat->dim[j].size;
						if( sz )
						{
							int t = idx/sz;
							ptr += (idx - t*sz)*mat->dim[j].step;
							idx = t;
						}
					}
				}
			}
			else if( CV_IS_SPARSE_MAT( arr ))
			{
				//CvSparseMat* m = (CvSparseMat*)arr;
				//if( m->dims == 1 )
				//	ptr = icvGetNodePtr( (CvSparseMat*)arr, &idx, _type, 1, 0 );
				//else
				//{
				//	int i, n = m->dims;
				//	int* _idx = (int*)cvStackAlloc(n*sizeof(_idx[0]));
		  //          
				//	for( i = n - 1; i >= 0; i-- )
				//	{
				//		int t = idx / m->size[i];
				//		_idx[i] = idx - t*m->size[i];
				//		idx = t;
				//	}
				//	ptr = icvGetNodePtr( (CvSparseMat*)arr, _idx, _type, 1, 0 );
				//}
			}
			else
			{
				CV_ERROR( CV_StsBadArg, "unrecognized or unsupported array type" );
			}

			__END__;

			return ptr;
		}


	};
}