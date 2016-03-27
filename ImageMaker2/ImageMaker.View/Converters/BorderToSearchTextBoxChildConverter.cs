using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ImageMaker.CommonView.Converters;
using ImageMaker.Themes.CustomControls;
using ImageMaker.View.Helpers;

namespace ImageMaker.View.Converters
{
    public class BorderToSearchTextBoxChildConverter: BaseConverter<BorderToSearchTextBoxChildConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DependencyObject parent = value as Border;
            if (parent == null)
                return DependencyProperty.UnsetValue;

            var child=parent.GetChildOfType<SearchBoxCtl>();
            return child.SearchText;
        }
    }
}
