using System;
using System.Drawing;
using Danilins.Multitouch.Logic.Properties;

namespace Danilins.Multitouch.Logic.LegacySupport.Cursors
{
	class ClickCursor : AnimationCursor
	{
		public ClickCursor()
		{
			ActionImage = Resources.ClickCircle0;
			FrameImages.Add(Resources.ClickCircle0);
			FrameImages.Add(Resources.ClickCircle1);
			FrameImages.Add(Resources.ClickCircle2);
			FrameImages.Add(Resources.ClickCircle3);
			FrameImages.Add(Resources.ClickCircle4);
			FrameImages.Add(Resources.ClickCircle5);
			FrameImages.Add(Resources.ClickCircle6);
			FrameImages.Add(Resources.ClickCircle7);
			FrameImages.Add(Resources.ClickCircle8);
			FrameImages.Add(Resources.ClickCircle9);
			FrameImages.Add(Resources.ClickCircle10);
			FrameImages.Add(Resources.ClickCircle11);
		}

		protected override int Interval
		{
			get { return 10; }
		}
	}
}