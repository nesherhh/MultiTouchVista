using System;
using System.Threading;
using Multitouch.Contracts;

namespace MultipleMice
{
	class MouseContact : Contact, ICloneable
	{
		private static int idCounter;

		internal IntPtr Handle { get; private set; }

		const int height = 10;
		const int width = 10;

		public MouseContact(MouseContact clone)
			: base(clone.Id, clone.State, clone.Position, clone.MajorAxis, clone.MinorAxis)
		{
			Handle = clone.Handle;
			Orientation = clone.Orientation;
		}

		public MouseContact(DeviceStatus state)
			: base(idCounter, ContactState.New, state.Location.ToPoint(), width, height)
		{
			Handle = state.Handle;

			Interlocked.Increment(ref idCounter);

			Orientation = 0;
		}

		internal void Update(DeviceStatus data)
		{
			Position = data.Location.ToPoint();

			if (data.ButtonState == DeviceState.Move)
				State = ContactState.Moved;
			else if (data.ButtonState == DeviceState.Up)
				State = ContactState.Removed;
		}

		public override string ToString()
		{
			return string.Format("ID: {0}, Position: {1}, State: {2}, Handle: {3}", Id, Position, State, Handle);
		}

		public object Clone()
		{
			return new MouseContact(this);
		}
	}
}