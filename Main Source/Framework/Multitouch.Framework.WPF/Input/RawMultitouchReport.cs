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

		public MultitouchDevice MultitouchDevice
		{
			get { return (MultitouchDevice)Device; }
		}

		public RawMultitouchReport(MultitouchDevice device, PresentationSource source, int id, double x, double y, double width, double height,
			ContactState state, int timestamp)
			: base(device, timestamp)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			InputSource = source;
			Contact = new Contact(device, id, x, y, width, height, state);
			RoutedEvent = MultitouchLogic.PreviewRawInputEvent;
		}

		public RawMultitouchReport(RawMultitouchReport copy)
			: base(copy.Device, copy.Timestamp - 1)
		{
			InputSource = copy.InputSource;
			Contact = copy.Contact;
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			RawMultitouchReportHandler handler = (RawMultitouchReportHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}