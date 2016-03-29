using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;
        private RelayCommand _manageTemplatesCommand;
        private RelayCommand _manageCompositionsCommand;
        private RelayCommand _manageAppSettingsCommand;
        private RelayCommand _manageCameraSettingsCommand;
        private RelayCommand _manageThemesCommand;
        private RelayCommand _showStatsCommand;
        private RelayCommand _changePasswordCommand;
        private RelayCommand _moduleManagedCommand;
        private bool _appSettingsVisible;

        public WelcomeViewModel(IViewModelNavigator navigator, SettingsProvider settingsProvider)
        {
            _navigator = navigator;
            _settingsProvider = settingsProvider;
        }

        public override void Initialize()
        {
            var moduleSettings = _settingsProvider.GetAvailableModules();
            _appSettingsVisible = moduleSettings.AvailableModules.Any();
        }

        public RelayCommand ChangePasswordCommand => _changePasswordCommand ?? (_changePasswordCommand = new RelayCommand(ShowChangePassword));

        public RelayCommand ManageAppSettingsCommand => _manageAppSettingsCommand ?? (_manageAppSettingsCommand = new RelayCommand(OpenSettingsExplorer,CanOpenSettingsExplorer));

        public RelayCommand ManageCameraSettingsCommand => _manageCameraSettingsCommand ?? (_manageCameraSettingsCommand = new RelayCommand(OpenCameraSettingsExplorer));

        public RelayCommand ManageTemplatesCommand => _manageTemplatesCommand ?? (_manageTemplatesCommand = new RelayCommand(OpenTemplateExplorer));

        public RelayCommand ManageCompositionsCommand => _manageCompositionsCommand ?? (_manageCompositionsCommand = new RelayCommand(OpenCompositionsExplorer));

        public RelayCommand ManageThemesCommand => _manageThemesCommand ?? (_manageThemesCommand = new RelayCommand(ManageThemes));

        public RelayCommand ShowStatsCommand => _showStatsCommand ?? (_showStatsCommand = new RelayCommand(ShowStats));

        public RelayCommand ModuleManagedCommand
            => _moduleManagedCommand ?? (_moduleManagedCommand = new RelayCommand(ModuleManaged));

        private bool CanOpenSettingsExplorer()
        {
            return _appSettingsVisible;
        }

        private void ModuleManaged()
        {
            _navigator.NavigateForward<ModuleManagedViewModel>(this, null);
        }

        private void OpenCameraSettingsExplorer()
        {
            _navigator.NavigateForward<CameraSettingsExplorerViewModel>(this, null);
        }

        private void ShowChangePassword()
        {
            _navigator.NavigateForward<ChangePasswordViewModel>(this, null);
        }

        private void ShowStats()
        {
            _navigator.NavigateForward<CurrentSessionViewModel>(this, null);
        }

        private void ManageThemes()
        {
            _navigator.NavigateForward<ThemeManagerViewModel>(this, null);
        }

        private void OpenSettingsExplorer()
        {
            _navigator.NavigateForward<AppSettingsExplorerViewModel>(this, null);
        }

        private void OpenCompositionsExplorer()
        {
            _navigator.NavigateForward<CompositionsExplorerViewModel>(this, null);
        }

        private void OpenTemplateExplorer()
        {
            _navigator.NavigateForward<TemplateExplorerViewModel>(this, null);
        }
    }
}
