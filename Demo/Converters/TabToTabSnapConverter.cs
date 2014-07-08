using Demo.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Demo.Converters
{
    /// <summary>
    /// This converter is to demonstrate how to dynamically choose what tabs can snap out to form new windows.
    /// </summary>
   public class TabToTabSnapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TabClass3)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
