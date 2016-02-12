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
            Dispatcher.UnhandledException += (sender, args) => {
                MessageBox.Show(args.Exception.ToString());
                LogManager.GetCurrentClassLogger().Error(args.Exception);
                MainViewModel.ShowAdminCommand.Execute(null);
            };
            InitApp();
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
            MainWindow = new MainWindow() {DataContext = mainViewModel};
            MainWindow.Closed += (o, args) =>
            {
                mainViewModel.Dispose();
                MainViewModel.ShowAdminCommand.Execute(null);
            };
            MainWindow.Show();

        }
    }
}
