using System;
using System.Windows;
using System.Windows.Controls;
using Danilins.Multitouch.Providers.Configuration.Models;

namespace Danilins.Multitouch.Providers.Configuration.View
{
	/// <summary>
	/// Interaction logic for FilterConfigurationWindow.xaml
	/// </summary>
	public partial class FilterConfigurationView : UserControl
	{
		public static readonly DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof (FilterConfigurationModel),
			typeof (FilterConfigurationView));

		public FilterConfigurationView()
		{
			InitializeComponent();
		}

		public FilterConfigurationModel Model
		{
			get { return (FilterConfigurationModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
	}
}
