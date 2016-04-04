using System.Windows;
using System.Windows.Controls;

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
            var s = (CustomPasswordBox) sender;
            s.Text = "";
            if (e.NewValue == null)
            {
                return;
            }
            var vl = e.NewValue.ToString();
            for (var i = 0; i < vl.Length; i++)
            {
                s.Text += "●";
            }
        }
    }
}
