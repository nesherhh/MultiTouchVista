using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WindowsFormsApplication1
{
	public partial class FormAForge : Form
	{
		IVideoSource device;
		FiltersSequence filtersSequence;
		FilterContext context;
		Timer timer;
		Bitmap bitmap;

		public FormAForge()
		{
			InitializeComponent();

			context = new FilterContext();
			filtersSequence = new FiltersSequence();
			filtersSequence.Add(new FilterVisualization(new GrayscaleY(), context));
			filtersSequence.Add(new FilterVisualization(new GaussianBlur(), context));
			filtersSequence.Add(new FilterVisualization(new ResizeBicubic(160, 120), context));

			filtersSequence.Add(new RemoveBackground(context));
			filtersSequence.Add(new Normalization(context));
			filtersSequence.Add(new Binarization(context));
			//filtersSequence.Add(new FilterVisualization(new Invert(), context));
			//filtersSequence.Add(new FilterVisualization(new Opening(), context));
			filtersSequence.Add(new Blobs(context));

			foreach (IFilter filter in filtersSequence)
			{
				FilterVisualization visualization = filter as FilterVisualization;
				if (visualization != null)
					flowLayoutPanelFilters.Controls.Add(visualization);
			}
		}

		void device_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			filtersSequence.Apply(eventArgs.Frame);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (device != null && device.IsRunning)
				device.Stop();
			base.OnClosing(e);
		}

		private void buttonStart_Click(object sender, EventArgs e)
		{
			//bitmap = (Bitmap)Bitmap.FromFile(@"d:\users\daniel\desktop\end.jpg");

			//timer = new Timer();
			//timer.Interval = 1000 / 30;
			//timer.Tick += timer_Tick;
			//timer.Start();

			FilterInfoCollection filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

			device = new VideoCaptureDevice(filterInfoCollection.OfType<FilterInfo>().Where(f => f.Name.Contains("SN9C105")).Single().MonikerString);
			//device = new FileVideoSource("d:\\reference 2.avi");
			device.NewFrame += device_NewFrame;
			device.Start();

			buttonStart.Enabled = false;
		}

		void timer_Tick(object sender, EventArgs e)
		{
			device_NewFrame(null, new NewFrameEventArgs(bitmap));
		}

		private void buttonBackground_Click(object sender, EventArgs e)
		{
			context.Background = null;
		}
	}
}