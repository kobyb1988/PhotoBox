using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ImageMaker.AdminViewModels.Ninject;
using ImageMaker.AdminViewModels.ViewModels;
using ImageMaker.CommonViewModels.Ninject;
using Ninject;

namespace ImageMaker.AdminView
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
