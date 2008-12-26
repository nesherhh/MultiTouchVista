using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Multitouch.Configuration.WPF.Models
{
	public class ConfigurationModel : DependencyObject
	{
		ConfigurationLogic logic;

		public static readonly DependencyPropertyKey AvailableProvidersPropertyKey = DependencyProperty.RegisterReadOnly("AvailableProviders",
			typeof(ObservableCollection<InputProvider>), typeof(ConfigurationModel), new PropertyMetadata(null));
		public static readonly DependencyProperty AvailableProvidersProperty = AvailableProvidersPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SelectedProviderProperty = DependencyProperty.Register("SelectedProvider", typeof(InputProvider),
			typeof(ConfigurationModel), new PropertyMetadata(OnSelectedProviderChanged));

		public static readonly DependencyProperty CurrentProviderProperty = DependencyProperty.Register("CurrentProvider", typeof(InputProvider),
			typeof(ConfigurationModel));

		static void OnSelectedProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((ConfigurationModel)d).OnSelectedProviderChanged(e);
		}

		public ICommand SetCurrentProvider { get; private set; }
		public ICommand RestartService { get; private set; }
		public ICommand ShowConfiguration { get; private set; }

		public InputProvider CurrentProvider
		{
			get { return (InputProvider)GetValue(CurrentProviderProperty); }
			set { SetValue(CurrentProviderProperty, value); }
		}

		public InputProvider SelectedProvider
		{
			get { return (InputProvider)GetValue(SelectedProviderProperty); }
			set { SetValue(SelectedProviderProperty, value); }
		}

		public ObservableCollection<InputProvider> AvailableProviders
		{
			get { return (ObservableCollection<InputProvider>)GetValue(AvailableProvidersProperty); }
			private set { SetValue(AvailableProvidersPropertyKey, value); }
		}

		public ConfigurationModel()
		{
			logic = new ConfigurationLogic();
			AvailableProviders = new ObservableCollection<InputProvider>();
			SetCurrentProvider = new DelegateCommand(OnSetCurrentProvider, OnCanSetCurrentProvider);
			RestartService = new DelegateCommand(OnRestartCommand);
			ShowConfiguration = new DelegateCommand(OnShowConfiguration, OnCanExecuteShowConfiguration);
		}

		bool OnCanExecuteShowConfiguration(object arg)
		{
			if(DesignerProperties.GetIsInDesignMode(this))
				return false;
			return logic.HasConfiguration;
		}

		void OnShowConfiguration(object obj)
		{
			WindowInteropHelper helper = new WindowInteropHelper(Application.Current.MainWindow);
			logic.ShowConfiguration(helper.Handle);
		}

		bool OnCanSetCurrentProvider(object arg)
		{
			return SelectedProvider != null;
		}

		void OnSetCurrentProvider(object obj)
		{
			CurrentProvider = logic.CurrentProvider = SelectedProvider;
		}

		void OnRestartCommand(object obj)
		{
			logic.RestartService();
		}

		public void Initialize()
		{
			foreach (InputProvider provider in logic.AvailableProviders)
				AvailableProviders.Add(provider);
			CurrentProvider = logic.CurrentProvider;
		}

		void OnSelectedProviderChanged(DependencyPropertyChangedEventArgs e)
		{
			((DelegateCommand)SetCurrentProvider).RaiseCanExecuteChanged();
		}
	}
}