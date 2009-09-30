using System;

namespace Multitouch.Framework.WPF
{
	static class Check
	{
		public static void NotNull(object argument, string parameter)
		{
			if (argument == null)
				throw new ArgumentNullException(parameter);
		}
	}
}
