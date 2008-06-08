using System;
using Multitouch.Contracts;

namespace DWMaxxAddIn
{
	class TransformedContact : IContact
	{
		public int Id { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public ContactState State { get; set; }

		public TransformedContact(IContact copy)
		{
			Id = copy.Id;
			X = copy.X;
			Y = copy.Y;
			Width = copy.Width;
			Height = copy.Height;
			State = copy.State;
		}
	}
}
