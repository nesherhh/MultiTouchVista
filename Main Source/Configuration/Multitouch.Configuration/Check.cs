using System;

namespace Multitouch.Configuration
{
	/// <summary>
	/// Various checks
	/// </summary>
	public static class Check
	{
		/// <summary>
		/// Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> <c>Null</c>.
		/// </summary>
		/// <param name="value">Value to be checked</param>
		/// <param name="name">Name of parameter</param>
		public static void NotNull(object value, string name)
		{
			if (value == null)
				throw new ArgumentNullException(name);
		}
	}
}
