using System;
using System.Windows;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	class TimeCircleCursorWPF : AnimationCursorWPF
	{
		public TimeCircleCursorWPF(Point location)
			: base(location)
		{
			for (int i = 0; i < 13; i++)
				FrameImages.Add("pack://application:,,,/Danilins.Multitouch.Logic;component/Resources/TimerCircle" + i + ".png");
			CloseOnEnd = false;
		}

		protected override TimeSpan Interval
		{
			get { return TimeSpan.FromMilliseconds(70); }
		}
	}
}