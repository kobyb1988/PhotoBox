using System;
using System.Globalization;
using System.Windows;

namespace ImageMaker.CommonView.Converters
{
    public class BoolToVisibleConverter:BaseConverter<BoolToVisibleConverter>
    {
       
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility notVisibleState = Visibility.Hidden;
            if (parameter != null && parameter.ToString().ToLower() == "Collapsed")
                notVisibleState = Visibility.Collapsed;

            if (value == DependencyProperty.UnsetValue || value == null) return notVisibleState;

            bool boolVal = (bool)value;
            

            if (parameter!=null && parameter.ToString().ToLower()== "invert")
                return boolVal ? notVisibleState : Visibility.Visible;
            return boolVal ? Visibility.Visible : notVisibleState;
        }
    }
}
