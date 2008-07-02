using System;
using System.Windows.Ink;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Information about gesture.
	/// </summary>
	public class GestureEventArgs : ContactEventArgs
	{
		/// <summary>
		/// Gets or sets the gesture.
		/// </summary>
		/// <value>The gesture.</value>
		public ApplicationGesture Gesture { get; private set; }

		internal GestureEventArgs(RawMultitouchGestureReport raw, int timestamp)
			: base(raw.MultitouchDevice, raw, timestamp)
		{
			Gesture = raw.Gesture;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GestureEventArgs"/> class.
		/// </summary>
		/// <param name="copy">The <see cref="Multitouch.Framework.WPF.Input.GestureEventArgs"/> instance containing the event data.</param>
		/// <param name="timestamp">The timestamp.</param>
		internal GestureEventArgs(GestureEventArgs copy, int timestamp)
			: base(copy.MultitouchDevice, copy.Contact, timestamp)
		{
			Gesture = copy.Gesture;
		}

		/// <summary>
		/// Invokes the event handler.
		/// </summary>
		/// <param name="genericHandler">The generic handler.</param>
		/// <param name="genericTarget">The generic target.</param>
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			GestureEventHandler handler = (GestureEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}