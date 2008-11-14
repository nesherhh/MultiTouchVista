using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TestApplication
{
	/// <summary>
	/// Interaction logic for TestUserControl.xaml
	/// </summary>
	public partial class TestUserControl : UserControl
	{
		public TestUserControl()
		{
			InitializeComponent();
		}

		public string Filename
		{
			get { return (string)GetValue(FilenameProperty); }
			set { SetValue(FilenameProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Filename.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty FilenameProperty =
			DependencyProperty.Register("Filename", typeof(string), typeof(TestUserControl), new PropertyMetadata(OnFilenameChanged));

		static void OnFilenameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((TestUserControl)d).OnFilenameChanged(e);
		}


		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(ImageSource), typeof(TestUserControl), new UIPropertyMetadata(null));


		void OnFilenameChanged(DependencyPropertyChangedEventArgs e)
		{
			string value = (string)e.NewValue;
			Source = BitmapFrame.Create(new Uri(value));
		}
	}
}