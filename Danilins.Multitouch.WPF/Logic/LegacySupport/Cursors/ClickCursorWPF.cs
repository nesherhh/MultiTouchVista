using System;
using System.Windows;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	class ClickCursorWPF : AnimationCursorWPF
	{
		public ClickCursorWPF(Point location)
			: base(location)
		{
			for (int i = 0; i < 12; i++)
				FrameImages.Add("pack://application:,,,/Danilins.Multitouch.Logic;component/Resources/ClickCircle" + i + ".png");
		}

		protected override TimeSpan Interval
		{
			get { return TimeSpan.FromMilliseconds(10); }
		}
	}
}