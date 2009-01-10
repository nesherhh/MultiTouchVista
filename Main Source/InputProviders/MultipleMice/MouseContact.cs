using System;
using System.Threading;
using System.Windows;
using Multitouch.Contracts;

namespace MultipleMice
{
	class MouseContact : IContactData, ICloneable
	{
		private static int idCounter;
		private readonly int id;

		public int Id
		{
			get { return id; }
		}

		internal IntPtr Handle { get; private set; }
		
		public Rect Bounds { get; private set; }
		public Point Position { get; private set; }
		public double Orientation { get; private set; }
		public double Area { get; private set; }
		public double MajorAxis { get; private set; }
		public double MinorAxis { get; private set; }
		public ContactState State { get; private set; }

		const int height = 10;
		const int width = 10;

		public MouseContact(MouseContact clone)
		{
			Area = clone.Area;
			Bounds = clone.Bounds;
			Handle = clone.Handle;
			id = clone.Id;
			MajorAxis = clone.MajorAxis;
			MinorAxis = clone.MinorAxis;
			Orientation = clone.Orientation;
			Position = clone.Position;
			State = clone.State;
		}

		public MouseContact(DeviceStatus state)
		{
			Handle = state.Handle;

			Interlocked.Increment(ref idCounter);
			id = idCounter;

			MajorAxis = 0;
			MinorAxis = 0;
			Orientation = 0;

			Position = state.Location.ToPoint();

			Bounds = new Rect(Position.X - width / 2d, Position.Y - height / 2d, width, height);
			Area = Bounds.Width * Bounds.Height;
			
			State = ContactState.New;
		}

		internal void Update(DeviceStatus data)
		{
			Position = data.Location.ToPoint();
			Bounds = new Rect(Position.X - width / 2d, Position.Y - height / 2d, width, height);

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