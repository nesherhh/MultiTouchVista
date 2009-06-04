using System;
using Multitouch.Framework.Input.Service;
using Point=System.Windows.Point;
using Rect=System.Windows.Rect;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// Represents some contact on the surface
	/// </summary>
	public class Contact
	{
		internal Contact(ContactData contact, IntPtr relativeTo, long timestamp)
		{
			Area = contact.Area;
			Bounds = contact.Bounds;
			Id = contact.Id;
			MajorAxis = contact.MajorAxis;
			MinorAxis = contact.MinorAxis;
			Orientation = contact.Orientation;
			Hwnd = contact.Hwnd;
			RelativeTo = relativeTo;
			Position = ConvertPosition(contact.Position);

			switch (contact.State)
			{
				case Service.ContactState.New:
					State = ContactState.New;
					break;
				case Service.ContactState.Removed:
					State = ContactState.Removed;
					break;
				case Service.ContactState.Moved:
					State = ContactState.Moved;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			Timestamp = timestamp;
		}

		private Point ConvertPosition(Point position)
		{
			if (!Hwnd.Equals(RelativeTo))
			{
				NativeMethods.POINT point = position.ToPOINT();
				NativeMethods.MapWindowPoints(Hwnd, RelativeTo, ref point, 1);
				return point.ToPoint();
			}
			return position;
		}

		/// <summary>
		/// Area of a contact
		/// </summary>
		/// <value>The area.</value>
		public double Area { get; private set; }

		/// <summary>
		/// Bounding box of a contact
		/// </summary>
		public Rect Bounds { get; private set; }

		/// <summary>
		/// Id of a contact
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
		/// Position
		/// </summary>
		public Point Position { get; private set; }

		/// <summary>
		/// Contact state
		/// </summary>
		public ContactState State { get; private set; }

		/// <summary>
		/// Coordinates are relative to this window
		/// </summary>
		public IntPtr RelativeTo { get; set; }

		/// <summary>
		/// Timestamp of frame
		/// </summary>
		public long Timestamp { get; private set; }

		/// <summary>
		/// Window Handle
		/// </summary>
		public IntPtr Hwnd { get; private set; }

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		public override string ToString()
		{
			return string.Format("Id: {0}, State: {1}, Position: {2}", Id, State, Position);
		}
	}
}