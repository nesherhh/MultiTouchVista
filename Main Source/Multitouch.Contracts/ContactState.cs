using System;

namespace Multitouch.Contracts.Contracts
{
	public enum ContactState
	{
		/// <summary>
		/// A new contact
		/// </summary>
		New,
		/// <summary>
		/// Contact removed
		/// </summary>
		Removed,
		/// <summary>
		/// Contact moved
		/// </summary>
		Moved
	}
}
