using System;
using System.Windows.Ink;

namespace Multitouch.Framework.WPF.Input
{
	class RawMultitouchGestureReport : RawMultitouchReport
	{
		public ApplicationGesture Gesture { get; private set; }

		public RawMultitouchGestureReport(RawMultitouchReport copy, ApplicationGesture gesture) : base(copy)
		{
			Gesture = gesture;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			RawMultitouchGestureReportHandler report = (RawMultitouchGestureReportHandler)genericHandler;
			report(genericTarget, this);
		}
	}
}