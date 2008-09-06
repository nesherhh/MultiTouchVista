using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using openCV;
using OpenCVCommon.Filters;
using OpenCVFilterLib;
using OpenCVTest.Filters;

namespace OpenCVTest
{
	public partial class FormOpenCV : Form
	{
		FilterPipline filterPipline;
		FilterContext context;
		CvCapture capture;
		Timer timer;

		public FormOpenCV()
		{
			InitializeComponent();

			context = new FilterContext();
			filterPipline = new FilterPipline();
			filterPipline.Add(new FilterVisualization(new Grayscale(context), context));
			filterPipline.Add(new FilterVisualization(new Blur(context), context));
			filterPipline.Add(new FilterVisualization(new Resize(context), context));
			filterPipline.Add(new RemoveBackgroundVisualization(context));
			filterPipline.Add(new FilterVisualization(new NormalizationFilter(), context));
			filterPipline.Add(new FilterVisualization(new ClosingFilter(), context));
			filterPipline.Add(new BlobsVisualization(context));
		
			foreach (IFilter filter in filterPipline)
			{
				FilterVisualization visualization = filter as FilterVisualization;
				if (visualization != null)
					flowLayoutPanelFilters.Controls.Add(visualization);
			}
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			if (capture.ptr != IntPtr.Zero)
				DestroyCapture();

			CreateCapture();

			timer = new Timer();
			timer.Tick += timer_Tick;
			if (context.Fps > 0)
				timer.Interval = (int)(1000 / context.Fps);
			else
				timer.Interval = 1000 / 30;
			timer.Start();

			buttonStart.Enabled = false;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			DestroyTimer();

			DestroyCapture();

			base.OnClosing(e);
		}

		void DestroyTimer()
		{
			if (timer != null)
				timer.Stop();
		}

		void DestroyCapture()
		{
			if(capture.ptr != IntPtr.Zero)
			{
				cvlib.CvReleaseCapture(ref capture);
				capture.ptr = IntPtr.Zero;
			}
		}

		void CreateCapture()
		{
			//capture = cvlib.CvCreateFileCapture("d:\\reference 3.avi");
			capture = cvlib.CvCreateCameraCapture(0);
			if (capture.ptr == IntPtr.Zero)
				throw new Exception("could not create capture");

			cvlib.CvQueryFrame(ref capture);
			context.Width = cvlib.cvGetCaptureProperty(capture, cvlib.CV_CAP_PROP_FRAME_WIDTH);
			context.Height = cvlib.cvGetCaptureProperty(capture, cvlib.CV_CAP_PROP_FRAME_HEIGHT);
			context.Fps = cvlib.cvGetCaptureProperty(capture, cvlib.CV_CAP_PROP_FPS);
		}

		void timer_Tick(object sender, EventArgs e)
		{
			IntPtr ptr = cvlib.CvQueryFrame(ref capture, false);
			if (ptr != IntPtr.Zero)
			{
				IplImage frame = (IplImage)Marshal.PtrToStructure(ptr, typeof(IplImage));
				frame.ptr = ptr;
				filterPipline.Apply(frame);
			}
			else
			{
				DestroyTimer();
				DestroyCapture();
				buttonStart.Enabled = true;
			}
		}

		private void buttonBackground_Click(object sender, EventArgs e)
		{
			cvlib.CvReleaseImage(ref context.Background);
		}
	}
}
