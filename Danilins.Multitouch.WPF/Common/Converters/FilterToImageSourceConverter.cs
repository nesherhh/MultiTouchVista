using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Danilins.Multitouch.Common.Filters;

namespace Danilins.Multitouch.Common.Converters
{
	public class FilterToImageSourceConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ResultData data = value as ResultData;
			if (data == null || targetType != typeof(ImageSource))
				return DependencyProperty.UnsetValue;

			PixelFormat format = PixelFormats.Bgr24;
			if(data.BitPerPixel == 8)
				format = PixelFormats.Gray8;
			return BitmapSource.Create(data.Width, data.Height, 96, 96, format, null, data.Pixels, format.GetStride(data.Width));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
