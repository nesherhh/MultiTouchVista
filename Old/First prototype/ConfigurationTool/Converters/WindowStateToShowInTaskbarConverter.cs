using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Danilins.Multitouch.ConfigurationTool.Converters
{
	class WindowStateToShowInTaskbarConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is WindowState)
			{
				WindowState state = (WindowState)value;
				switch (state)
				{
					case WindowState.Maximized:
						return true;
					case WindowState.Minimized:
						return false;
					case WindowState.Normal:
						return true;
					default:
						return DependencyProperty.UnsetValue;
				}
			}
			else
				return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
