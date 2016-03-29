using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Dto;
using ImageMaker.Common.Enums;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;

namespace ImageMaker.ViewModels.ViewModels
{
    public class SelectActivityViewModel : BaseViewModel
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;

        private RelayCommand _importPatternsCommand;
        private RelayCommand _proceedToPatternSelectionCommand;
        private RelayCommand _instagramSurfCommand;
        private bool _isPrinterVisible;
        private RelayCommand _checkPrintingStatusCommand;
        private bool _selfyBoxVisible;
        private bool _instaBoxVisible;

        public SelectActivityViewModel(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
        }

        public override void Initialize()
        {
            ModuleSettingDto moduleSettings = _settingsProvider.GetAvailableModules();
            if (moduleSettings != null)
            {
                SelfyBoxVisible = moduleSettings.AvailableModules.Any(x => x == AppModules.SelfyBox);
                InstaBoxVisible = moduleSettings.AvailableModules.Any(x => x == AppModules.InstaBox);
            }

            AppSettingsDto settings = _settingsProvider.GetAppSettings();

            IsPrinterVisible = settings != null && settings.ShowPrinterOnStartup;
        }

        public bool IsPrinterVisible
        {
            get { return _isPrinterVisible; }
            set
            {
                if (_isPrinterVisible == value)
                    return;

                _isPrinterVisible = value;
                RaisePropertyChanged();
            }
        }

        public bool SelfyBoxVisible
        {
            get { return _selfyBoxVisible; }
            set { Set(() => SelfyBoxVisible, ref _selfyBoxVisible, value); }
        }

        public bool InstaBoxVisible
        {
            get { return _instaBoxVisible; }
            set { Set(() => InstaBoxVisible, ref _instaBoxVisible, value); }
        }

        public RelayCommand ImportPatternsCommand => _importPatternsCommand ?? (_importPatternsCommand = new RelayCommand(ImportPatterns));

        public RelayCommand ProceedToPatternSelectionCommand => _proceedToPatternSelectionCommand ?? (_proceedToPatternSelectionCommand = new RelayCommand(ProceedToPatternSelection));

        public RelayCommand InstagramSurfCommand => _instagramSurfCommand ?? (_instagramSurfCommand = new RelayCommand(InstagramSurf));


        public RelayCommand CheckPrintingStatusCommand => _checkPrintingStatusCommand ?? (_checkPrintingStatusCommand = new RelayCommand(CheckPrintingStatus));

        private void CheckPrintingStatus()
        {
            _navigator.NavigateForward<PrinterActivityViewerViewModel>(this, null);
        }

        private void InstagramSurf()
        {
            _navigator.NavigateForward<InstagramExplorerViewModel>(this, null);
        }

        private void ProceedToPatternSelection()
        {
            _navigator.NavigateForward<SelectPatternViewModel>(this, null);
        }

        private void ImportPatterns()
        {
            _navigator.NavigateForward<ImportPatternsViewModel>(this, null);
        }
    }
}
