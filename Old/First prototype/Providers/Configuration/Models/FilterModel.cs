using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using Danilins.Multitouch.Common.Filters;

namespace Danilins.Multitouch.Providers.Configuration.Models
{
	public class FilterModel:DependencyObject
	{
		private readonly IFilter filter;

		public static readonly DependencyProperty LastResultProperty = DependencyProperty.Register("LastResult", typeof(ResultData),
			typeof(FilterModel));

		public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(ObservableCollection<FrameworkElement>),
			typeof(FilterModel));
	
		public FilterModel(IFilter filter)
		{
			SetValue(PropertiesProperty, new ObservableCollection<FrameworkElement>());
			this.filter = filter;
			InitializeProperties();
		}

		public void Refresh()
		{
			LastResult = filter.LastResult;
		}

		public ResultData LastResult
		{
			get { return (ResultData)GetValue(LastResultProperty); }
			set { SetValue(LastResultProperty, value); }
		}

		public string Name
		{
			get { return filter.GetType().Name; }
		}

		public ObservableCollection<FrameworkElement> Properties
		{
			get { return (ObservableCollection<FrameworkElement>)GetValue(PropertiesProperty); }
			set { SetValue(PropertiesProperty, value); }
		}

		private void InitializeProperties()
		{
			foreach (PropertyInfo propertyInfo in filter.GetType().GetProperties())
			{
				Type propertyType = propertyInfo.PropertyType;
				FrameworkElement propertyControl = PropertyControlFactory.CreateControl(filter, propertyType, propertyInfo);
				if(propertyControl != null)
					Properties.Add(propertyControl);
			}
		}
	}
}
