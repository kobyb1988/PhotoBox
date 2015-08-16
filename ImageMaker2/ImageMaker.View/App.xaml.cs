using System.Windows;
using ImageMaker.CommonViewModels.Ninject;
using ImageMaker.ViewModels.Ninject;
using ImageMaker.ViewModels.ViewModels;
using Ninject;

namespace ImageMaker.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Dispatcher.UnhandledException += (sender, args) => MessageBox.Show(args.Exception.ToString());
            InitApp();
        }

        private void InitApp()
        {
            var kernel = NinjectBootstrapper.GetKernel(new MainModule(), new NinjectBaseModule());
            MainViewModel mainViewModel = kernel.Get<MainViewModel>();
            MainWindow = new MainWindow() { DataContext = mainViewModel };
            MainWindow.Closed += (o, args) => mainViewModel.Dispose();
            MainWindow.Show();
        }
    }
}
