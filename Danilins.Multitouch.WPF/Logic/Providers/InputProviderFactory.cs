using System;
using System.Configuration;
using System.Reflection;
using Danilins.Multitouch.Common.Providers;

namespace Danilins.Multitouch.Logic.Providers
{
	public static class InputProviderFactory
	{

		public static IInputProvider Create(Guid id)
		{
			Type foundProviderType = null;

			foreach (Type type in LogicUtility.ProvidersAssembly.GetTypes())
			{
				InputProviderAttribute[] attributes = (InputProviderAttribute[])type.GetCustomAttributes(typeof(InputProviderAttribute), false);
				if (attributes.Length > 0)
				{
					if (attributes[0].Id.Equals(id))
					{
						foundProviderType = type;
						break;
					}
				}
			}

			if (foundProviderType != null)
			{
				ConstructorInfo constructor = foundProviderType.GetConstructor(Type.EmptyTypes);
				return (IInputProvider)constructor.Invoke(null);
			}
			throw new ConfigurationErrorsException(string.Format("Could not find a provider with Id: '{0}'", id));			
		}
	}
}