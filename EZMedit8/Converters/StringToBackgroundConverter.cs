using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EZMedit8.Converters
{
    public class StringToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string || string.IsNullOrEmpty(value.ToString()) || !File.Exists(value.ToString())) 
            { value = "pack://application:,,,/Assets/Background%20Images/default_background.jpg"; }
            ImageBrush imageBrush = new(new BitmapImage(new Uri(value.ToString(), UriKind.RelativeOrAbsolute)));
            return imageBrush.ImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
