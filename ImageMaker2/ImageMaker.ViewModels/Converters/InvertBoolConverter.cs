using System;
using System.Globalization;

namespace ImageMaker.ViewModels.Converters
{
    public class InvertBoolConverter:BaseConverter<InvertBoolConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }
    }
}
