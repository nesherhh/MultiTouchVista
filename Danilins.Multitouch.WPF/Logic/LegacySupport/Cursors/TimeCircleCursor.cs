using System;
using System.Drawing;
using Danilins.Multitouch.Logic.Properties;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	class TimeCircleCursor : AnimationCursor
	{
		public TimeCircleCursor()
		{
			ActionImage = Resources.TimerCircle0;
			FrameImages.Add(Resources.TimerCircle0);
			FrameImages.Add(Resources.TimerCircle1);
			FrameImages.Add(Resources.TimerCircle2);
			FrameImages.Add(Resources.TimerCircle3);
			FrameImages.Add(Resources.TimerCircle4);
			FrameImages.Add(Resources.TimerCircle5);
			FrameImages.Add(Resources.TimerCircle6);
			FrameImages.Add(Resources.TimerCircle7);
			FrameImages.Add(Resources.TimerCircle8);
			FrameImages.Add(Resources.TimerCircle9);
			FrameImages.Add(Resources.TimerCircle10);
			FrameImages.Add(Resources.TimerCircle11);
			FrameImages.Add(Resources.TimerCircle12);
			CloseOnEnd = false;
		}

		protected override int Interval
		{
			get { return 70; }
		}
	}
}