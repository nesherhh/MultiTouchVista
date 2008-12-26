using System;
using System.Windows;
using Multitouch.Contracts;

namespace TuioProvider
{
	class TuioContact : IContactData
	{
		public TuioContact(int id, double x, double y, double width, double height, double angle, Rect bounds, ContactState state)
		{
			Id = id;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			Angle = angle;
			Bounds = bounds;
			State = state;
		}

		public int Id { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }
		public double Angle { get; private set; }
		public Rect Bounds { get; private set; }
	}
}