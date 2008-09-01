using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using openCV;

namespace WindowsFormsApplication1
{
	public partial class Form1 : Form
	{
		CvCapture camera;
		Timer timer;
		IplImage firstFrame;
		IplImage currentFrame;
		IplImage maskImage;

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (timer != null)
				DestroyCamera();

			timer = new Timer();
			timer.Tick += timer_Tick;
			timer.Interval = 1000 / 60;

			camera = cvlib.CvCreateCameraCapture(0);
			if (camera.ptr == IntPtr.Zero)
				throw new Exception("could not create camera");
			
			IplImage frame = cvlib.CvQueryFrame(ref camera);


			double width = cvlib.cvGetCaptureProperty(camera, cvlib.CV_CAP_PROP_FRAME_WIDTH);
			double height = cvlib.cvGetCaptureProperty(camera, cvlib.CV_CAP_PROP_FRAME_HEIGHT);

			firstFrame = cvlib.CvCreateImage(new CvSize((int)width, (int)height), (int)cvlib.IPL_DEPTH_8U, 1);
            cvlib.CvCvtColor(ref frame, ref firstFrame, cvlib.CV_BGR2GRAY);

			pictureBox1.Size = new Size((int)width, (int)height);


			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			if(pictureBox1.Image != null)
				pictureBox1.Image.Dispose();

			IplImage frame = cvlib.CvQueryFrame(ref camera);
			cvlib.CvFlip(ref frame, ref frame, 0);

			if (currentFrame.ptr == IntPtr.Zero)
				currentFrame = cvlib.CvCreateImage(new CvSize(frame.width, frame.height), (int)cvlib.IPL_DEPTH_8U, 1);
			cvlib.CvCvtColor(ref frame, ref currentFrame, cvlib.CV_BGR2GRAY);

			TransformImage();

			cvlib.CvCvtColor(ref currentFrame, ref frame, cvlib.CV_GRAY2BGR);

			pictureBox1.Image = cvlib.ToBitmap(frame, false);
		}

		void TransformImage()
		{
			if(maskImage.ptr == IntPtr.Zero)
			{
				maskImage = cvlib.CvCreateImage(new CvSize(firstFrame.width, firstFrame.height), (int)cvlib.IPL_DEPTH_8U, 1);
				cvlib.CvSet(ref maskImage, new CvScalar(1, 1, 1, 1));
			}
			// remove background
			//cvlib.CvSub(ref firstFrame, ref currentFrame, ref currentFrame, ref maskImage);

			// normalize
			CvHistogram hist = cvlib.CvCreateHist(1, new[] { 256 }, cvlib.CV_HIST_ARRAY, IntPtr.Zero, 1);
			cvlib.CvCalcHist(new[] { currentFrame.ptr }, ref hist, 0);
			cvlib.CvNormalizeHist(ref hist, 20000);
			cvlib.CvCalcBackProject(ref currentFrame, ref currentFrame, ref hist);
			cvlib.CvReleaseHist(ref hist);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			DestroyCamera();

			base.OnClosing(e);
		}

		void DestroyCamera()
		{
			if(timer != null)
			{
				timer.Stop();
				timer = null;
			}

			if (camera.ptr != IntPtr.Zero)
			{
				cvlib.CvReleaseCapture(ref camera);
				camera.ptr = IntPtr.Zero;
			}
		}
	}
}
