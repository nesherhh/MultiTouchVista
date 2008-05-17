using System;
using System.Windows.Ink;

namespace Multitouch.Framework.WPF.Input
{
	public class GestureEventArgs : ContactEventArgs
	{
		public ApplicationGesture Gesture { get; private set; }

		internal GestureEventArgs(RawMultitouchGestureReport raw, int timestamp)
			: base(raw.MultitouchDevice, raw, timestamp)
		{
			Gesture = raw.Gesture;
		}

		public GestureEventArgs(GestureEventArgs copy, int timestamp)
			: base(copy.MultitouchDevice, copy.Contact, timestamp)
		{
			Gesture = copy.Gesture;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			GestureEventHandler handler = (GestureEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}