using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.Common.Filters;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.Providers;
using Danilins.Multitouch.Providers.Configuration.Commands;

namespace Danilins.Multitouch.Providers.Configuration.Models
{
	public class FilterConfigurationModel:DependencyObject
	{
		private IConfigurableInputProvider inputProvider;

		public CommandModel AcceptCommand { get; private set; }
		public CommandModel CancelCommand { get; private set; }

		public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register("Filters", typeof(ObservableCollection<FilterModel>),
			typeof(FilterConfigurationModel));
		
		public FilterConfigurationModel(IConfigurableInputProvider inputProvider)
		{
			this.inputProvider = inputProvider;
			AcceptCommand = new AcceptFilterConfigurationCommand(this);
			Filters = new ObservableCollection<FilterModel>();

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				BitmapProvider provider = inputProvider as BitmapProvider;
				if (provider != null)
				{
					foreach (IFilter filter in provider.Filters)
						Filters.Add(new FilterModel(filter));
					provider.FilterUpdated += NewFrame;
				}
			}
		}

		void NewFrame(object sender, EventArgs e)
		{
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
         	{
         		foreach (FilterModel model in Filters)
         			model.Refresh();
         	});
		}

		public ObservableCollection<FilterModel> Filters
		{
			get { return (ObservableCollection<FilterModel>)GetValue(FiltersProperty); }
			set { SetValue(FiltersProperty, value); }
		}

		internal void Save()
		{
			inputProvider.Save();
		}
	}
}
