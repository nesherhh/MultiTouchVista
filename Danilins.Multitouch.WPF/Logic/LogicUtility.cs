using System;
using System.IO;
using System.Reflection;

namespace Danilins.Multitouch.Logic
{
	static class LogicUtility
	{
		private static Assembly providersAssembly;

		public static Assembly ProvidersAssembly
		{
			get
			{
				if (providersAssembly == null)
				{
					string providersAssemblyFilename = "Danilins.Multitouch.Providers";
					providersAssembly = AppDomain.CurrentDomain.Load(providersAssemblyFilename);
					if (providersAssembly == null)
						throw new FileNotFoundException("Could not load assembly", providersAssemblyFilename);
				}
				return providersAssembly;
			}
		}
	}
}
