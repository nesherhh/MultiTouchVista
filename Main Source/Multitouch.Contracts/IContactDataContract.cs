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
		Rect Bounds{get;}

		/// <summary>
		/// X coordinate of contact center.
		/// </summary>
		double X { get; }
		
		/// <summary>
		/// Y coordinate of contact center.
		/// </summary>
		double Y { get; }

		/// <summary>
		/// Width of contact.
		/// </summary>
		double Width { get; }
		
		/// <summary>
		/// Height of contact.
		/// </summary>
		double Height { get; }

		/// <summary>
		/// Angle
		/// </summary>
		double Angle { get; }
		
		/// <summary>
		/// State of contact. See <see cref="ContactState"/>
		/// </summary>
		ContactState State { get; }
	}
}