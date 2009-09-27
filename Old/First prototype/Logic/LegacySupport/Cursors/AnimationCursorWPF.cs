using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	abstract class AnimationCursorWPF : CursorWPF
	{
		DispatcherTimer timer;
		int imageIndex;
		StringCollection frameImages;
		bool repeatForever;
		private bool closeOnEnd;

		public AnimationCursorWPF(Point location)
			: base(location)
		{
			repeatForever = false;
			closeOnEnd = true;
			frameImages = new StringCollection();
			timer = new DispatcherTimer();
			timer.Interval = Interval;
			timer.Tick += timer_Tick;
			imageIndex = 0;

			timer.Start();
		}

		protected abstract TimeSpan Interval
		{
			get;
		}

		public StringCollection FrameImages
		{
			get { return frameImages; }
		}

		public bool RepeatForever
		{
			get { return repeatForever; }
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			image.Source = new BitmapImage(new Uri(FrameImages[imageIndex]));
			if (IsClosing)
				timer.Stop();
			else
			{
				if (imageIndex == FrameImages.Count - 1)
				{
					if (RepeatForever)
						imageIndex = 0;
					else
					{
						timer.Stop();
						if (CloseOnEnd)
							Close();
					}
				}
				else
					imageIndex++;
			}
		}

		public bool CloseOnEnd
		{
			get { return closeOnEnd; }
			set { closeOnEnd = value; }
		}
	}
}