using System;
using System.Windows;
using System.Windows.Input;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	class RawMultitouchReport : InputEventArgs
	{
		public PresentationSource InputSource { get; private set; }
		public Contact Contact { get; private set; }
		public MultitouchDevice MultitouchDevice { get; private set; }

		public RawMultitouchReport(MultitouchDevice device, PresentationSource source, int id, double x, double y, double width, double height,
			ContactState state, int timestamp)
			: base(device, timestamp)
		{
			InputSource = source;
			Contact = new Contact(id, x, y, width, height, state);
			MultitouchDevice = device;
			RoutedEvent = MultitouchLogic.PreviewRawInputEvent;
		}

		public RawMultitouchReport(RawMultitouchReport copy)
			: this(copy.MultitouchDevice, copy.InputSource, copy.Contact.Id, copy.Contact.X, copy.Contact.Y, copy.Contact.Width,
			copy.Contact.Height, copy.Contact.State, copy.Timestamp - 1)
		{
			Contact.SetElement(copy.Contact.Element);
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			RawMultitouchReportHandler handler = (RawMultitouchReportHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}