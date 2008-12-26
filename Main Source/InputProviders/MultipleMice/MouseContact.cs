using System;
using System.Threading;
using System.Windows;
using Multitouch.Contracts;
using Point=System.Drawing.Point;

namespace MultipleMice
{
	class MouseContact : IContactData
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
		public double Angle { get { return 0; } }
		public Rect Bounds { get; private set; }

		public MouseContact(DeviceStatus state)
		{
			Handle = state.Handle;

			Interlocked.Increment(ref idCounter);
			id = idCounter;

			Height = 10;
			Width = 10;

			Point location = state.Location;
			X = location.X;
			Y = location.Y;

			Bounds = new Rect(X, Y, Width, Height);

			State = ContactState.New;
		}

		internal void Update(DeviceStatus data)
		{
			Point location = data.Location;
			X = location.X;
			Y = location.Y;
			Bounds = new Rect(X, Y, Width, Height);

			if (data.ButtonState == DeviceState.Move)
				State = ContactState.Moved;
			else if (data.ButtonState == DeviceState.Up)
				State = ContactState.Removed;
		}

		public override string ToString()
		{
			return string.Format("ID: {0}, XY: {1};{2} W:{3}, H:{4}, State: {5}, Handle: {6}", Id, X, Y, Width, Height, State, Handle);
		}
	}
}