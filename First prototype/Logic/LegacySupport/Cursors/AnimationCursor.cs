using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	abstract class AnimationCursor : Cursor
	{
		Timer timer;
		int imageIndex;
		List<Image> frameImages;
		bool repeatForever;
		private bool closeOnEnd;

		public AnimationCursor()
		{
			repeatForever = false;
			closeOnEnd = true;
			frameImages = new List<Image>();
			timer = new Timer();
			timer.Interval = Interval;
			timer.Tick += timer_Tick;
			imageIndex = 0;
			timer.Start();
		}

		protected abstract int Interval
		{
			get;
		}

		public List<Image> FrameImages
		{
			get { return frameImages; }
		}

		public bool RepeatForever
		{
			get { return repeatForever; }
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			ActionImage = new Bitmap(FrameImages[imageIndex]);
			if (IsClosing)
				timer.Stop();
			else
			{
				if (imageIndex == FrameImages.Count - 1)
				{
					if (RepeatForever)
					{
						imageIndex = 0;
						UpdateLayered();
					}
					else
					{
						timer.Stop();
						if (CloseOnEnd)
							Close();
					}
				}
				else
				{
					imageIndex++;
					UpdateLayered();
				}
			}
		}

		public bool CloseOnEnd
		{
			get { return closeOnEnd; }
			set { closeOnEnd = value; }
		}
	}
}