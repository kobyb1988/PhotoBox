using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ImageMaker.AdminViewModels.Ninject;
using ImageMaker.AdminViewModels.ViewModels;
using ImageMaker.CommonViewModels.Ninject;
using Ninject;
using NLog;

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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            Dispatcher.UnhandledException += (sender, args) =>
            {
                MessageBox.Show(args.Exception.ToString());
                LogManager.GetCurrentClassLogger().Error(args.Exception);
            };
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
