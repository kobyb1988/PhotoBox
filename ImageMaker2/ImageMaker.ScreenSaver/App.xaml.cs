using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ImageMaker.CommonViewModels.Ninject;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.ScreenSaver.Ninject;
using Ninject;
using NLog;

namespace ImageMaker.ScreenSaver
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

            if (!Debugger.IsAttached)
                Debugger.Launch();
            Debugger.Break();
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
            SettingsProvider settings = kernel.Get<SettingsProvider>();

            Task.Run(async () =>
            {
                var appset = settings.GetAppSettings();
                if (appset != null)
                {
                    while (appset.DateEnd>DateTime.Now)
                    {
                        await Task.Delay(5000);
                    }
                    Current.Dispatcher.Invoke(StopWnd);
                }
            });
            MainWindow = new MainWindow() ;
            MainWindow.ShowDialog();
        }

        private void StopWnd()
        {
            MainWindow.Close();
        }
    }
}
