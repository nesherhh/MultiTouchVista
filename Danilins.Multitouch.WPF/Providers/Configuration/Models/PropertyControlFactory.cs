using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Danilins.Multitouch.Common.Filters;

namespace Danilins.Multitouch.Providers.Configuration.Models
{
	class PropertyControlFactory
	{
		public static FrameworkElement CreateControl(IFilter filter, Type propertyType, PropertyInfo propertyInfo)
		{
			FrameworkElement result = null;
			if (propertyType == typeof (int))
				result = CreateControlForInt32(filter, propertyInfo);

			return result;
		}

		private static FrameworkElement CreateControlForInt32(IFilter filter, PropertyInfo propertyInfo)
		{
			Slider slider = new Slider();
			slider.DataContext = filter;
			slider.TickPlacement = TickPlacement.BottomRight;

			RangeAttribute[] attributes = (RangeAttribute[])propertyInfo.GetCustomAttributes(typeof(RangeAttribute), false);
			if (attributes != null && attributes.Length > 0)
			{
				RangeAttribute range = attributes[0];
				slider.Minimum = range.Minimum;
				slider.Maximum = range.Maximum;
				slider.IsSnapToTickEnabled = true;
				slider.TickFrequency = 1;
			}

			Panel panel = new DockPanel();
			
			Label labelName = new Label();
			labelName.Content = propertyInfo.Name;
			panel.Children.Add(labelName);

			Label labelValue = new Label();
			labelValue.DataContext = slider;
			labelValue.MinWidth = 30;
			Binding bindingToValue = new Binding("Value");
			bindingToValue.Mode = BindingMode.OneWay;
			labelValue.SetBinding(Label.ContentProperty, bindingToValue);
			panel.Children.Add(labelValue);

			panel.Children.Add(slider);
			slider.SetBinding(Slider.ValueProperty, propertyInfo.Name);
			return panel;
		}
	}
}
