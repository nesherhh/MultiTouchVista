using System;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	public class Contact
	{
		public int Id { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }

		public Contact(int id, double x, double y, double width, double height, ContactState state)
		{
			Id = id;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			State = state;
		}
	}
}
