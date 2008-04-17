using System;
using System.Windows;

namespace Danilins.Multitouch.Common.Providers
{
	public interface IConfigurableInputProvider:IInputProvider
	{
		FrameworkElement GetConfigurationUI();
		void Save();
	}
}
