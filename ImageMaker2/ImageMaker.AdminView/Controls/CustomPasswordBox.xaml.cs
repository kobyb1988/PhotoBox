using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageMaker.AdminView.Controls
{
    /// <summary>
    /// Interaction logic for CustomPasswordBox.xaml
    /// </summary>
    public partial class CustomPasswordBox : TextBox
    {
        public CustomPasswordBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password",
            typeof(string), typeof(CustomPasswordBox), new FrameworkPropertyMetadata("", OnPasswordChange));

        public string Password
        {
            set { SetValue(PasswordProperty, value); }
            get { return GetValue(PasswordProperty).ToString(); }
        }

        private static void OnPasswordChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = ((CustomPasswordBox)sender);
            s.Text = "";
            if (e.NewValue == null)
            {
                return;
            }
            var vl = e.NewValue.ToString();
            for (int i = 0; i < vl.Length; i++)
            {
                s.Text += "●";
            }
        }
    }
}
