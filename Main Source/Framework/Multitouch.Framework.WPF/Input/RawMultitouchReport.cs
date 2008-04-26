using System;
using System.Windows;
using System.Windows.Input;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	class RawMultitouchReport : InputEventArgs
	{
		public PresentationSource InputSource { get; private set; }
		public int Id { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }
		public MultitouchDevice MultitouchDevice { get; private set; }

		public RawMultitouchReport(MultitouchDevice device, PresentationSource source, int id, double x, double y, double width, double height,
			ContactState state, int timestamp)
			: base(device, timestamp)
		{
			InputSource = source;
			Id = id;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			State = state;
			MultitouchDevice = device;
			RoutedEvent = MultitouchLogic.PreviewRawInputEvent;
		}

		public RawMultitouchReport(RawMultitouchReport copy)
			: this(copy.MultitouchDevice, copy.InputSource, copy.Id, copy.X, copy.Y, copy.Width, copy.Height, copy.State, copy.Timestamp - 1)
		{
		}
	}
}