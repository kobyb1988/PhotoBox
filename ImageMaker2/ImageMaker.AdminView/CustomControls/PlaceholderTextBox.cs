using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageMaker.AdminView.CustomControls
{
    public class PlaceholderTextBox: TextBox
    {
        public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.Register("PlaceHolder", typeof(string), typeof(PlaceholderTextBox),new PropertyMetadata((o,e)=> { ((PlaceholderTextBox)o).SetPlaceHolder(); }));

        public string PlaceHolder
        {
            set { SetValue(PlaceHolderProperty, value); }
            get { return (string)GetValue(PlaceHolderProperty); }
        }

        public PlaceholderTextBox()
        {
            GotFocus += GotFocusText;
            LostFocus += LostFocusText;
        }

        private void SetPlaceHolder()
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = PlaceHolder;
            }
        }

        private void LostFocusText(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                SetPlaceHolder();
        }

        private void GotFocusText(object sender, RoutedEventArgs e)
        {
            if (Text == PlaceHolder)
                Text = string.Empty;
        }
    }

}
