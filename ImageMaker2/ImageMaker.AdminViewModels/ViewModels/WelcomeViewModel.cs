using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private RelayCommand _manageTemplatesCommand;
        private RelayCommand _manageCompositionsCommand;
        private RelayCommand _manageAppSettingsCommand;
        private RelayCommand _manageCameraSettingsCommand;
        private RelayCommand _manageThemesCommand;
        private RelayCommand _showStatsCommand;

        public WelcomeViewModel(
            IViewModelNavigator navigator
            )
        {
            _navigator = navigator;
        }

        public RelayCommand ManageAppSettingsCommand
        {
            get { return _manageAppSettingsCommand ?? (_manageAppSettingsCommand = new RelayCommand(OpenSettingsExplorer)); }
        }

        public RelayCommand ManageCameraSettingsCommand
        {
            get { return _manageCameraSettingsCommand ?? (_manageCameraSettingsCommand = new RelayCommand(OpenCameraSettingsExplorer)); }
        }

        private void OpenCameraSettingsExplorer()
        {
            _navigator.NavigateForward<CameraSettingsExplorerViewModel>(this, null);
        }

        public RelayCommand ManageTemplatesCommand
        {
            get { return _manageTemplatesCommand ?? (_manageTemplatesCommand = new RelayCommand(OpenTemplateExplorer)); }
        }

        public RelayCommand ManageCompositionsCommand
        {
            get { return _manageCompositionsCommand ?? (_manageCompositionsCommand = new RelayCommand(OpenCompositionsExplorer)); }
        }

        public RelayCommand ManageThemesCommand
        {
            get { return _manageThemesCommand ?? (_manageThemesCommand = new RelayCommand(ManageThemes)); }
        }

        public RelayCommand ShowStatsCommand
        {
            get { return _showStatsCommand ?? (_showStatsCommand = new RelayCommand(ShowStats)); }
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
