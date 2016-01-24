using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ChromeTabs.Converters
{
    public class IsLessThanConverter : DependencyObject,IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double param = System.Convert.ToDouble(parameter);
            double width = System.Convert.ToDouble(value);

            if (width > 0 && width < param)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
