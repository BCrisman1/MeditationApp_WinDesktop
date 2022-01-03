using EZMedit8.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace EZMedit8.Converters
{
    public class IntervalModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IntervalMode || parameter is not IntervalMode) { return Visibility.Collapsed; }
            if ((IntervalMode)value == (IntervalMode)parameter) { return Visibility.Visible; }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
