using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ImageMaker.Themes;

namespace SandBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<int> list = new List<int> { 1, 2, 3, 4, 5 };

            CollectionView = CollectionViewSource.GetDefaultView(list);
        }

        public ICollectionView CollectionView { get; set; }

        //private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ThemeManager.Change(Color.FromArgb(255, 0, 255, 255));
        //}
    }
}
