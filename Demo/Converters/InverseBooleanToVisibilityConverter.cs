using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Demo.Converters
{
    public class InverseBooleanToVisibilityConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BooleanToVisibilityConverter().Convert(!(bool)value,targetType,parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
