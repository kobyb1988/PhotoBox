using System.Windows;
using System.Windows.Controls;
using ImageMaker.AdminViewModels.ViewModels;
using ImageMaker.CommonView.Helpers;
using ImageMaker.CommonViewModels.Providers;

namespace ImageMaker.AdminView.Controls
{
    /// <summary>
    /// Interaction logic for WelcomeCtl.xaml
    /// </summary>
    public partial class WelcomeCtl : UserControl
    {
        public WelcomeCtl()
        {
            InitializeComponent();
            var mainViewModel = (MainViewModel)Application.Current.MainWindow?.DataContext;
            mainViewModel?.UpdateSessionStart();
        }
    }
}
