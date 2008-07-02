using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Represents a contact.
	/// </summary>
	public class Contact : IEquatable<Contact>
	{
		readonly MultitouchDevice device;

		/// <summary>
		/// Contact Id
		/// </summary>
		public int Id { get; private set; }
		/// <summary>
		/// Center X coordinate
		/// </summary>
		public double X { get; private set; }
		/// <summary>
		/// Center Y coordinate
		/// </summary>
		public double Y { get; private set; }
		/// <summary>
		/// Center coordinates
		/// </summary>
		public Point Position { get; private set; }
		/// <summary>
		/// Width of contact
		/// </summary>
		public double Width { get; private set; }
		/// <summary>
		/// Height of contact
		/// </summary>
		public double Height { get; private set; }
		/// <summary>
		/// Contacts state
		/// </summary>
		public ContactState State { get; private set; }
		/// <summary>
		/// Element under contact
		/// </summary>
		public UIElement Element { get; private set; }

		internal Contact(MultitouchDevice device, int id, double x, double y, double width, double height, ContactState state)
		{
			this.device = device;
			Id = id;
			X = x;
			Y = y;
			Position = new Point(x, y);
			Width = width;
			Height = height;
			State = state;
		}

		internal Contact(RawMultitouchReport report)
			: this(report.MultitouchDevice, report.Contact.Id, report.Contact.X, report.Contact.Y, report.Contact.Width, report.Contact.Height, report.Contact.State)
		{ }

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
		public override bool Equals(object obj)
		{
			Contact other = obj as Contact;
			return Equals(other);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		public bool Equals(Contact other)
		{
			if (other == null)
				return false;
			return Id.Equals(other.Id);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		internal void SetElement(UIElement element)
		{
			Element = element;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("Id: {0}, X,Y: {1},{2}, W,H: {3},{4}, State: {5}", Id, X, Y, Width, Height, State);
		}

		/// <summary>
		/// Returns all coordinates history of this contact
		/// </summary>
		public IList<Point> ContactHistory
		{
			get { return device.ContactHistory; }
		}

		/// <summary>
		/// Returns previous coordinates of contact
		/// </summary>
		public Point PreviousPosition
		{
			get
			{
				if (device.ContactHistory.Count > 2)
					return device.ContactHistory[device.ContactHistory.Count - 2];
				return device.ContactHistory.Last();
			}
		}
	}
}