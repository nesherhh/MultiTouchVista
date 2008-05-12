using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Commands;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.ConfigurationTool.Commands;
using Danilins.Multitouch.Logic;

namespace Danilins.Multitouch.ConfigurationTool.Models
{
	public class MainModel:DependencyObject
	{
		public CommandModel StartMultitouch { get; private set; }
		public CommandModel StopMultitouch { get; private set; }
		public CommandModel ShowConfiguration { get; private set; }
		public CommandModel EnableOnStart { get; private set; }

		public static readonly DependencyProperty IsMultitouchRunningProperty = DependencyProperty.Register("IsMultitouchRunning", typeof(bool),
			typeof(MainModel));

		public static readonly DependencyProperty ProvidersProperty = DependencyProperty.Register("Providers",
			typeof(ObservableCollection<InputProviderModel>), typeof(MainModel), new UIPropertyMetadata(new ObservableCollection<InputProviderModel>()));

		public static readonly DependencyProperty SelectedProviderProperty = DependencyProperty.Register("SelectedProvider",
			typeof(InputProviderModel), typeof(MainModel), new PropertyMetadata(SelectedProviderChanged));

		public static readonly DependencyProperty IsEnabledOnStartProperty = DependencyProperty.Register("IsEnabledOnStart", typeof(bool),
			typeof(MainModel), new UIPropertyMetadata(false));

		private MainLogic mainLogic;

		public bool IsMultitouchRunning
		{
			get { return (bool)GetValue(IsMultitouchRunningProperty); }
			set { SetValue(IsMultitouchRunningProperty, value); }
		}

		public ObservableCollection<InputProviderModel> Providers
		{
			get { return (ObservableCollection<InputProviderModel>)GetValue(ProvidersProperty); }
			set { SetValue(ProvidersProperty, value); }
		}

		public InputProviderModel SelectedProvider
		{
			get { return (InputProviderModel)GetValue(SelectedProviderProperty); }
			set { SetValue(SelectedProviderProperty, value); }
		}

		public bool IsEnabledOnStart
		{
			get { return (bool)GetValue(IsEnabledOnStartProperty); }
			set { SetValue(IsEnabledOnStartProperty, value); }
		}

		internal MainLogic Logic
		{
			get { return mainLogic; }
		}

		public MainModel()
		{
			mainLogic = MainLogic.Instance;
			mainLogic.UIDispatcher = Dispatcher;

			IsMultitouchRunning = false;
			StartMultitouch = new StartMultitouchCommand(this);
			StopMultitouch = new StopMultitouchCommand(this);
			ShowConfiguration = new ShowConfigurationCommand(this);
			EnableOnStart = new EnableOnStartCommand(this);

			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				App.Current.Exit += Current_Exit;
				App.Current.MainWindow.StateChanged += MainWindow_StateChanged;
				InitializeProviderCollection();
				SetPreview();

				if (mainLogic.ConfigurationLogic.Section.EnableOnStart)
					StartMultitouch.OnExecute(null, null);
			}
		}

		void MainWindow_StateChanged(object sender, EventArgs e)
		{
			SetPreview();
		}

		private void SetPreview()
		{
			if (App.Current.MainWindow.WindowState == WindowState.Minimized)
				mainLogic.InputProviderLogic.ShowPreview = false;
			else
				mainLogic.InputProviderLogic.ShowPreview = true;
		}

		private static void SelectedProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MainModel mainModel = (MainModel)d;
			mainModel.SelectedProviderChanged(e);
		}

		void Current_Exit(object sender, ExitEventArgs e)
		{
			StopMultitouch.OnExecute(null, null);
			mainLogic.ConfigurationLogic.Save();
		}

		//internal void InputHandler(int width, int height, int bitPerPixel, byte[] pixels)
		//{
		//    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
		//    {
		//        InputImage = GetImageSource(width, height, bitPerPixel, pixels);
		//    });
		//}

		//internal void OutputHandler(int width, int height, int bitPerPixel, byte[] pixels)
		//{
		//    Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate
		//    {
		//        OutputImage = GetImageSource(width, height, bitPerPixel, pixels);
		//    });
		//}

		//private BitmapSource GetImageSource(int width, int height, int bitPerPixel, byte[] pixels)
		//{
		//    PixelFormat pixelFormat = PixelFormats.Bgr24;
		//    if (bitPerPixel == 8)
		//        pixelFormat = PixelFormats.Gray8;
		//    if (bitPerPixel == 32)
		//        pixelFormat = PixelFormats.Bgr32;
		//    return BitmapSource.Create(width, height, 96, 96, pixelFormat, null, pixels, pixelFormat.GetStride(width));
		//}

		private void SelectedProviderChanged(DependencyPropertyChangedEventArgs e)
		{
			InputProviderModel newValue = (InputProviderModel)e.NewValue;
			mainLogic.InputProviderLogic.SelectedProviderId = newValue.Id;
		}

		private void InitializeProviderCollection()
		{
			InputProviderLogic providerLogic = mainLogic.InputProviderLogic;
			foreach (InputProviderInfo pair in providerLogic.AvailableProviders)
			{
				InputProviderModel model = new InputProviderModel(pair.Id, pair.Name, pair.HasConfiguration);
				Providers.Add(model);
				if (model.Id == providerLogic.SelectedProviderId)
					SelectedProvider = model;
			}
		}
	}
}