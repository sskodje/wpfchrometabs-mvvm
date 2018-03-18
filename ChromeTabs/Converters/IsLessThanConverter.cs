using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChromeTabs.Converters
{
    public class IsLessThanConverter : DependencyObject,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double param = System.Convert.ToDouble(parameter);
            double width = System.Convert.ToDouble(value);

            if (width > 0 && width < param)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
