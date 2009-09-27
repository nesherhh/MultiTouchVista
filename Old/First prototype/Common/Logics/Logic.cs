using System;
using System.ComponentModel.Design;

namespace Danilins.Multitouch.Common.Logics
{
	public class Logic : IServiceContainer
	{
		ServiceContainer serviceContainer;

		public Logic()
		{
			serviceContainer = new ServiceContainer();
		}

		public Logic(IServiceProvider parentProvider)
		{
			serviceContainer = new ServiceContainer(parentProvider);
		}

		public T GetLogic<T>()
		{
			return (T)serviceContainer.GetService(typeof(T));
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			return serviceContainer.GetService(serviceType);
		}

		void IServiceContainer.AddService(Type serviceType, object serviceInstance)
		{
			serviceContainer.AddService(serviceType, serviceInstance);
		}

		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
		{
			serviceContainer.AddService(serviceType, serviceInstance, promote);
		}

		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			serviceContainer.AddService(serviceType, callback);
		}

		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			serviceContainer.AddService(serviceType, callback, promote);
		}

		void IServiceContainer.RemoveService(Type serviceType)
		{
			serviceContainer.RemoveService(serviceType);
		}

		void IServiceContainer.RemoveService(Type serviceType, bool promote)
		{
			serviceContainer.RemoveService(serviceType, promote);
		}
	}
}