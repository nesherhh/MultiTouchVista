using System;
using System.Windows;
using System.Windows.Input;

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

		public RawMultitouchReport(MultitouchDevice device, PresentationSource source, Framework.Input.Contact contact)
			: base(device, (int)contact.Timestamp)
		{
			if (device == null)
				throw new ArgumentNullException("device");

			InputSource = source;
			RoutedEvent = MultitouchLogic.PreviewRawInputEvent;
			Contact = new Contact(device, contact);
		}

		public RawMultitouchReport(RawMultitouchReport copy)
			: base(copy.Device, copy.Timestamp - 1)
		{
			InputSource = copy.InputSource;
			Contact = copy.Contact;
		}

		public Point GetPosition(IInputElement relativeTo)
		{
			return MultitouchDevice.GetPosition(relativeTo);
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			RawMultitouchReportHandler handler = (RawMultitouchReportHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}