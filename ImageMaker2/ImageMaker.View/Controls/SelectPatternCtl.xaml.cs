using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageMaker.ViewModels.ViewModels;
using ImageMaker.ViewModels.ViewModels.Images;

namespace ImageMaker.View.Controls
{
    /// <summary>
    /// Interaction logic for SelectPatternCtl.xaml
    /// </summary>
    public partial class SelectPatternCtl : UserControl
    {
        public SelectPatternCtl()
        {
            InitializeComponent();
        }

//        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
//        {
//#if BadTaughtScreen
//            ((ToggleButton)sender).MouseEnter += LinkToClickCommand;
//#endif
//        }
//#if BadTaughtScreen

//        private void LinkToClickCommand(object sender, MouseEventArgs e)
//        {
//            ((SelectPatternViewModel)DataContext).SelectedPattern = (TemplateViewModel)((FrameworkElement)e.Source).DataContext;
//        }
//#endif
    }
}
