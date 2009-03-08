using System;
using System.Windows;

namespace Multitouch.Contracts
{
	public class Contact
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Contact"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="state">The state.</param>
		/// <param name="position">The position.</param>
		/// <param name="majorAxis">The major axis.</param>
		/// <param name="minorAxis">The minor axis.</param>
		public Contact(int id, ContactState state, Point position, double majorAxis, double minorAxis)
		{
			Id = id;
			Position = position;
			State = state;
			MajorAxis = majorAxis;
			MinorAxis = minorAxis;
		}

		/// <summary>
		/// Contact Id.
		/// </summary>
		public int Id { get; private set; }

		/// <summary>
		/// State of contact. See <see cref="ContactState"/>
		/// </summary>
		public ContactState State { get; protected set; }

		/// <summary>
		/// Position of contact
		/// </summary>
		public Point Position { get; protected set; }

		public double MajorAxis { get; private set; }

		public double MinorAxis { get; private set; }

		/// <summary>
		/// Contact bounds
		/// </summary>
		public virtual Rect Bounds
		{
			get { return new Rect(Position.X - (MajorAxis / 2), Position.Y - (MinorAxis / 2), MajorAxis, MinorAxis); }
		}

		/// <summary>
		/// Orientation
		/// </summary>
		public double Orientation { get; set; }

		/// <summary>
		/// Area
		/// </summary>
		public virtual double Area
		{
			get { return MajorAxis * MinorAxis; }
		}
	}
}