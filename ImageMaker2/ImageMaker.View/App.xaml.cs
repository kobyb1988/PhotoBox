using System;
using System.Windows;
using ImageMaker.Common.Dto;
using ImageMaker.CommonViewModels.Ninject;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.Themes;
using ImageMaker.ViewModels.Ninject;
using ImageMaker.ViewModels.ViewModels;
using Ninject;
using NLog;
using System.Diagnostics;
using System.Windows.Threading;

namespace ImageMaker.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainViewModel MainViewModel;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += (sender, args) =>
            {
                HandleGlobalException("Ошибка в единственном UI Dispatcher потоке", args.Exception);
            };
            InitApp();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            HandleGlobalException(string.Format($"Ошибка от всех потоков домена приложения: {args.ExceptionObject.ToString()}"));
        }

        private void HandleGlobalException(string message, Exception exception = null)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (exception != null)
                logger.Error(exception, "Глобальная ошибка!");
            else
                logger.Error($"Глобальная ошибка: {message}");

            MessageBox.Show(message);
            MainViewModel.ShowAdminCommand.Execute(null);
        }

        private void InitApp()
        {
            var kernel = NinjectBootstrapper.GetKernel(new MainModule(), new NinjectBaseModule());

            var settings = kernel.Get<SettingsProvider>();


            ThemeSettingsDto theme = settings.GetThemeSettings();
            if (theme != null)
            {
                foreach (var property in typeof(ThemeSettingsDto).GetProperties())
                {
                    Properties.Add(property.Name, property.GetValue(theme));
                }
            }
            MainViewModel mainViewModel = kernel.Get<MainViewModel>();
            MainViewModel = mainViewModel;
            MainWindow = new MainWindow() { DataContext = mainViewModel };
            MainWindow.Closed += (o, args) =>
            {
                mainViewModel.Dispose();
                MainViewModel.ShowAdminCommand.Execute(null);
            };
            MainWindow.Show();

        }
    }
}
