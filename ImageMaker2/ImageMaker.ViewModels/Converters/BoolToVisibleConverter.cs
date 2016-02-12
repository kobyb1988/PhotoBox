using System;
using System.Globalization;
using System.Windows;

namespace ImageMaker.ViewModels.Converters
{
    public class BoolToVisibleConverter:BaseConverter<BoolToVisibleConverter>
    {
        public bool InvertValue { get; set; }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue || value == null) return Visibility.Hidden;

            bool boolVal = (bool)value;
            if (InvertValue)
                return boolVal ? Visibility.Hidden : Visibility.Visible;
            return boolVal ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
