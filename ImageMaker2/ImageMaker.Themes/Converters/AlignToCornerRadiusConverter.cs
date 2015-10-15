using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ImageMaker.Themes.CustomControls;

namespace ImageMaker.Themes.Converters
{
    public class AlignToCornerRadiusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                throw new ArgumentException(string.Format("not enough arguments, expected 2, but {0} present", values.Length));

            Align align;
            Enum.TryParse(values[0].ToString(), out align);
            double val = 0.0;
            double.TryParse(values[1].ToString(), out val);
            CornerRadius radius = new CornerRadius();
            
            if ((align & Align.TopRight) == Align.TopRight)
            {
                radius.TopRight = val;
            }

            if ((align & Align.TopLeft) == Align.TopLeft)
            {
                radius.TopLeft = val;
            }

            if ((align & Align.BottomRight) == Align.BottomRight)
            {
                radius.BottomRight = val;
            }

            if ((align & Align.BottomLeft) == Align.BottomLeft)
            {
                radius.BottomLeft = val;
            }

            return radius;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
