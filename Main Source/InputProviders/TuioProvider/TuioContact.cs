using System;
using Multitouch.Contracts;

namespace TuioProvider
{
	class TuioContact : IContact
	{
		public TuioContact(int id, double x, double y, double width, double height, ContactState state)
		{
			Id = id;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			State = state;
		}

		public int Id { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }
	}
}