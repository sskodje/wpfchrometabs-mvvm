using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ChromeTabs.Converters
{
    public class TabPersistBehaviorToItemHolderVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TabPersistBehavior behavior = (TabPersistBehavior)value;
            switch (behavior)
            {
                default:
                case TabPersistBehavior.None:
                    return Visibility.Collapsed;
                case TabPersistBehavior.All:
                case TabPersistBehavior.Timed:
                    return Visibility.Visible;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
