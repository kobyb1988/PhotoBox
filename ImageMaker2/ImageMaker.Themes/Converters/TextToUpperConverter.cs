using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageMaker.Themes.Converters
{
    //public class TextToUpperConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value == null || parameter == null)
    //            return null;

    //        bool capitalize = true;
    //        bool.TryParse(parameter.ToString(), out capitalize);
    //        return !capitalize ? value : value.ToString().ToUpperInvariant();
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class TextToUpperConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return null;

            string value = values[0].ToString();
            bool capitalize = true;
            bool.TryParse(values[1].ToString(), out capitalize);
            return !capitalize ? value : value.ToString().ToUpperInvariant();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
