using System;
using System.Windows;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	public class Contact : IEquatable<Contact>
	{
		public int Id { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }
		public Point Position { get; private set; }
		public double Width { get; private set; }
		public double Height { get; private set; }
		public ContactState State { get; private set; }
		public UIElement Element { get; private set; }

		internal Contact(int id, double x, double y, double width, double height, ContactState state)
		{
			Id = id;
			X = x;
			Y = y;
			Position = new Point(x, y);
			Width = width;
			Height = height;
			State = state;
		}

		internal Contact(RawMultitouchReport report)
			: this(report.Contact.Id, report.Contact.X, report.Contact.Y, report.Contact.Width, report.Contact.Height, report.Contact.State)
		{ }

		public override bool Equals(object obj)
		{
			Contact other = obj as Contact;
			return Equals(other);
		}

		public bool Equals(Contact other)
		{
			if (other == null)
				return false;
			return Id.Equals(other.Id);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		internal void SetElement(UIElement element)
		{
			Element = element;
		}

		public override string ToString()
		{
			return string.Format("Id: {0}, X,Y: {1},{2}, W,H: {3},{4}, State: {5}", Id, X, Y, Width, Height, State);
		}
	}
}