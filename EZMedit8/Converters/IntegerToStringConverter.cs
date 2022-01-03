using System;
using System.Globalization;
using System.Windows.Data;


namespace EZMedit8.Converters
{
    public class IntegerToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int ? ((int)value).ToString() : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string ? ((int.TryParse(value.ToString(), out int intValue)) ? intValue : 0) : 0;
        }
    }
}
