using System;
using System.Globalization;
using System.Windows;

namespace ImageMaker.CommonView.Converters
{
    public class BoolToVisibleConverter:BaseConverter<BoolToVisibleConverter>
    {
       
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null) return Visibility.Hidden;

            bool boolVal = (bool)value;
            if (parameter!=null && parameter.ToString().ToLower()== "invert")
                return boolVal ? Visibility.Hidden : Visibility.Visible;
            return boolVal ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
