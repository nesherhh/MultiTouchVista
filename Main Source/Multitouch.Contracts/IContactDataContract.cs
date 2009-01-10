using System;
using System.AddIn.Contract;
using System.Windows;

namespace Multitouch.Contracts.Contracts
{
	public interface IContactDataContract : IContract
	{
		/// <summary>
		/// Contact Id.
		/// </summary>
		int Id { get; }
		
		/// <summary>
		/// Contact bounds
		/// </summary>
		Rect Bounds { get; }

		/// <summary>
		/// Position of contact
		/// </summary>
		Point Position { get; }

		/// <summary>
		/// Orientation
		/// </summary>
		double Orientation { get; }

		/// <summary>
		/// Area
		/// </summary>
		double Area { get; }

        double MajorAxis { get; }
		double MinorAxis { get; }
		
		/// <summary>
		/// State of contact. See <see cref="ContactState"/>
		/// </summary>
		ContactState State { get; }
	}
}