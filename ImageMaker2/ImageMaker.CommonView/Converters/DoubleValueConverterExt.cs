using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ImageMaker.Common.Extensions;

namespace ImageMaker.CommonView.Converters
{
    public class DoubleValueConverterExt : IMultiValueConverter
    {
        public double MinVal { get; set; }

        public double DefVal { get; set; }

        public double MaxVal { get; set; }

        public DoubleValueConverterExt()
        {
            MinVal = 0;
            DefVal = 0;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() < 2)
                throw new ArgumentException();

            double val = 0;
            double maxVal = 0;
            double result = DefVal;

            if (double.TryParse(values[0].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out val)
                && double.TryParse(values[1].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out maxVal))
            {
                MaxVal = maxVal;
                result = val < 1 && val > 0
                    ? val
                    : DefVal;
            }

            //todo 2 converters
            if (targetType == typeof (double))
                return (result * maxVal).TwoDigits();

            return result.ToString(CultureInfo.InvariantCulture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            double val = 0;
            if (value == null || !double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out val) || val >= 1 || val <= 0)
                return new object[] { Binding.DoNothing, Binding.DoNothing };
            
            return new object[] { (val).TwoDigits() };
        }
    }
}
