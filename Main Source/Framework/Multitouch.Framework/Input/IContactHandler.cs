using System;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// Contacts handler. 
	/// </summary>
	public interface IContactHandler
	{
		/// <summary>
		/// Processes the contact change.
		/// </summary>
		/// <param name="id">Id of contact.</param>
		/// <param name="x">X coordinate.</param>
		/// <param name="y">Y coordinate.</param>
		/// <param name="width">Width of contact.</param>
		/// <param name="height">Height of contact.</param>
		/// <param name="state">State of contact.</param>
		void ProcessContactChange(int id, double x, double y, double width, double height, ContactState state);
	}
}
