using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;


namespace EZMedit8.Converters
{
    public class GetFileNameFromPathStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string) { return null; }
            string filePath = value.ToString();
            return IsValidPath(filePath) ? Path.GetFileName(filePath) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool IsValidPath(string filePath)
        {
            try { return File.Exists(filePath); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }

            return false;
        }
    }
}
