using System;
using System.Globalization;
using System.Windows.Data;
using ImageMaker.CommonView.Helpers;

namespace ImageMaker.CommonView.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] buffer = value as byte[];
            if (buffer == null)
                return null;

            return buffer.ToImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
