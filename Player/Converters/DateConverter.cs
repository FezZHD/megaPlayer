using System;
using System.Globalization;
using System.Windows.Data;

namespace Player.Converters
{
    public class DateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertableString = (double) value;
            var returnString = TimeSpan.FromSeconds(convertableString).ToString("m\\:ss");
            return returnString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}