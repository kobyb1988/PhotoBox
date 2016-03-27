using System;
using System.Globalization;

namespace ImageMaker.CommonView.Converters
{
    public class TestBindingConverter:BaseConverter<TestBindingConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
