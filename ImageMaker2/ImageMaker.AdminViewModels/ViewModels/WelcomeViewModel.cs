using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IChildrenViewModelsFactory _welcomeViewModelChildFactory;
        private RelayCommand _manageTemplatesCommand;
        private RelayCommand _manageCompositionsCommand;
        private RelayCommand _manageAppSettingsCommand;
        private RelayCommand _manageCameraSettingsCommand;

        public WelcomeViewModel(
            IViewModelNavigator navigator,
            IChildrenViewModelsFactory welcomeViewModelChildFactory
            )
        {
            _navigator = navigator;
            _welcomeViewModelChildFactory = welcomeViewModelChildFactory;
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
            _navigator.NavigateForward(this, _welcomeViewModelChildFactory.GetChild<CameraSettingsExplorerViewModel>(null));
        }

        public RelayCommand ManageTemplatesCommand
        {
            get { return _manageTemplatesCommand ?? (_manageTemplatesCommand = new RelayCommand(OpenTemplateExplorer)); }
        }

        public RelayCommand ManageCompositionsCommand
        {
            get { return _manageCompositionsCommand ?? (_manageCompositionsCommand = new RelayCommand(OpenCompositionsExplorer)); }
        }

        private void OpenSettingsExplorer()
        {
            _navigator.NavigateForward(this, _welcomeViewModelChildFactory.GetChild<AppSettingsExplorerViewModel>(null));
        }

        private void OpenCompositionsExplorer()
        {
            _navigator.NavigateForward(this, _welcomeViewModelChildFactory.GetChild<CompositionsExplorerViewModel>(null));
        }

        private void OpenTemplateExplorer()
        {
            _navigator.NavigateForward(this, _welcomeViewModelChildFactory.GetChild<TemplateExplorerViewModel>(null));
        }
    }
}
