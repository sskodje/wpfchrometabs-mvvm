using System;
using System.Globalization;
using System.Windows.Data;
using Demo.ViewModel;

namespace Demo.Converters
{
    /// <summary>
    /// This converter is to demonstrate how to dynamically choose what tabs can snap out to form new windows.
    /// </summary>
   public class TabToTabSnapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabClass3)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
