using System;
using System.Windows;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	class RawMultitouchReport : InputEventArgs
	{
		public ContactContext Context { get; set; }
        public CaptureMode CaptureState { get; set; }
		public DependencyObject Captured { get; set; }
        public bool CleanUp { get; set; }

		public RawMultitouchReport(ContactContext contactContext)
			: base(null, (int)contactContext.Contact.Timestamp)
		{
			RoutedEvent = MultitouchLogic.PreviewRawInputEvent;
			Context = contactContext;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			RawMultitouchReportHandler handler = (RawMultitouchReportHandler)genericHandler;
			handler(genericTarget, this);
		}

		public RawMultitouchReport Clone()
		{
			return new RawMultitouchReport(Context.Clone());
		}

		public void UpdateFromPrevious(RawMultitouchReport previousReport)
		{
			Context.OverElement = previousReport.Context.OverElement;
			Context.ElementsList = previousReport.Context.ElementsList;
			CaptureState = previousReport.CaptureState;
			Captured = previousReport.Captured;
			Context.History = previousReport.Context.History;
			Context.History.Insert(0, previousReport.Context.Contact);
		}
	}
}