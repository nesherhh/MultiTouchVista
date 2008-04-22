using System;
using System.Threading;
using MultipleMice.Native;
using Multitouch.Contracts;

namespace MultipleMice
{
	class MouseContact : IContact
	{
		private static int idCounter;
		private readonly int id;

		public int Id
		{
			get { return id; }
		}

		internal IntPtr Handle { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }
		public double Width { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }

		public MouseContact(DeviceState state)
		{
			Handle = state.Handle;

			Interlocked.Increment(ref idCounter);
			id = idCounter;

			Height = 10;
			Width = 10;

			X = state.X;
			Y = state.Y;

			State = ContactState.New;
		}

		internal void Update(DeviceState data)
		{
			X = data.X;
			Y = data.Y;

			if(data.ButtonState == MouseButtonState.None)
				State = ContactState.Moved;
			else if(data.ButtonState == MouseButtonState.LeftUp)
				State = ContactState.Removed;
		}

		public override string ToString()
		{
			return string.Format("ID: {0}, XY: {1};{2} W:{3}, H:{4}, State: {5}, Handle: {6}", Id, X, Y, Width, Height, State, Handle);
		}
	}
}