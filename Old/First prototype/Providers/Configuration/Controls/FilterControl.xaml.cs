using System;
using System.Windows;
using Danilins.Multitouch.Providers.Configuration.Models;

namespace Danilins.Multitouch.Providers.Configuration.Controls
{
	public partial class FilterControl
	{
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(FilterModel), typeof(FilterControl));

		public FilterControl()
		{
			InitializeComponent();
		}

		public FilterModel Model
		{
			get { return (FilterModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
	}
}