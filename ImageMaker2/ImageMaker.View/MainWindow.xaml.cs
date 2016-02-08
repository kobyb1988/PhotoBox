using System.Windows;
using System.Windows.Input;
using ImageMaker.ViewModels.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ImageMaker.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TouchPoint _touchStart;
        private const byte Delta=100;

        public MainWindow()
        {
            InitializeComponent();

            //TouchDown += BasePage_TouchDown;
            //TouchMove += BasePage_TouchMove;
        }

        //void BasePage_TouchDown(object sender, TouchEventArgs e)
        //{
        //    _touchStart = e.GetTouchPoint(this);
        //}

        //void BasePage_TouchMove(object sender, TouchEventArgs e)
        //{
        //    var touch = e.GetTouchPoint(this);

        //    if (_touchStart != null && touch.Position.X > (_touchStart.Position.X - Width- Delta))
        //    {
        //        if (((MainViewModel)DataContext).ShowAdminCommand.CanExecute(sender))
        //            ((MainViewModel)DataContext).ShowAdminCommand.Execute(sender);
        //    }
        //    e.Handled = true;
        //}
    }
}
