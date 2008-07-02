using System;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// State of contact
	/// </summary>
	public enum ContactState
	{
		/// <summary>
		/// New contact
		/// </summary>
		New = 0,
		/// <summary>
		/// Contact was removed since last event.
		/// </summary>
		Removed = 1,
		/// <summary>
		/// Contact was moved since last event.
		/// </summary>
		Moved = 2
	}
}