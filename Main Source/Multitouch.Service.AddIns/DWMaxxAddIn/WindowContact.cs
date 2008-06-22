using System;
using System.Windows;
using Multitouch.Contracts;

namespace DWMaxxAddIn
{
	class WindowContact : IContact
	{
		Window owner;

		public WindowContact(IContact copy)
		{
			Id = copy.Id;
			Position = new Point(copy.X, copy.Y);
			Width = copy.Width;
			Height = copy.Height;
			State = copy.State;
		}

		public void Update(IContact contact)
		{
			if (Id != contact.Id)
				throw new ArgumentException("Id is not equal");
			PreviousPosition = Position;
			Position = new Point(contact.X, contact.Y);
            Width = contact.Width;
			Height = contact.Height;
			State = contact.State;
		}

		public int Id
		{
			get;
			private set;
		}

		public double X
		{
			get { return Position.X; }
		}

		public double Y
		{
			get { return Position.Y; }
		}

		public double Width
		{
			get;
			private set;
		}

		public double Height
		{
			get;
			private set;
		}

		public ContactState State
		{
			get;
			private set;
		}

		public Point Position { get; private set; }

		public Point PreviousPosition { get; private set; }

		public void SetOwner(Window owner)
		{
			this.owner = owner;
		}
	}
}
