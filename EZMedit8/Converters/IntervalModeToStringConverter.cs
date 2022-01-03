using EZMedit8.Enums;
using System;
using System.Globalization;
using System.Windows.Data;


namespace EZMedit8.Converters
{
    public class IntervalModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IntervalMode) { return null; }
            string mode = (IntervalMode)value switch
            {
                IntervalMode.Count => "Interval Count",
                                 _ => "Interval Delay",
            };

            return mode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
