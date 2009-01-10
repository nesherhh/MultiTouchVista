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
		/// Area
		/// </summary>
		public double Area { get; private set; }
        /// <summary>
		/// Bounding box
		/// </summary>
		public Rect Bounds { get; private set; }
		/// <summary>
		/// Contact Id
		/// </summary>
		public int Id { get; private set; }
		/// <summary>
		/// Major axis for an ellipse
		/// </summary>
		public double MajorAxis { get; private set; }
		/// <summary>
		/// Minor axis for an ellipse
		/// </summary>
		public double MinorAxis { get; private set; }
		/// <summary>
		/// Orientation
		/// </summary>
		public double Orientation { get; private set; }
		/// <summary>
		/// Center coordinates
		/// </summary>
		public Point Position { get; private set; }
		/// <summary>
		/// Contacts state
		/// </summary>
		public ContactState State { get; private set; }
		/// <summary>
		/// Timestamp of frame
		/// </summary>
		public long Timestamp { get; private set; }
		/// <summary>
		/// Element under contact
		/// </summary>
		public UIElement Element { get; private set; }

		internal Contact(MultitouchDevice multitouchDevice, Framework.Input.Contact contact)
		{
			device = multitouchDevice;
			Area = contact.Area;
			Bounds = contact.Bounds;
			Id = contact.Id;
			MajorAxis = contact.MajorAxis;
			MinorAxis = contact.MinorAxis;
			Orientation = contact.Orientation;
			Position = contact.Position;
			State = contact.State;
			Timestamp = contact.Timestamp;
		}

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
			return string.Format("Id: {0}, Position: {1}, State: {2}", Id, Position, State);
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