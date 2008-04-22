using System;
using Multitouch.Contracts;

namespace TouchLib
{
	class TouchLibContact : IContact
	{
		public int Id { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }

		public TouchLibContact(int id, double x, double y, double width, double height, ContactState state)
		{
			Height = height;
			Id = id;
			State = state;
			Width = width;
			X = x;
			Y = y;
		}
	}
}