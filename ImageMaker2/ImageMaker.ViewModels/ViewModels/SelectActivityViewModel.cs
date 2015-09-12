using System;
using System.Data.Entity.Infrastructure;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
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

        public SelectActivityViewModel(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
        }

        public override void Initialize()
        {
            AppSettingsDto settings = _settingsProvider.GetAppSettings();

            if (settings != null)
            {
                IsPrinterVisible = settings.ShowPrinterOnStartup;
            }
            else
            {
                IsPrinterVisible = false;
            }
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

        public RelayCommand ImportPatternsCommand
        {
            get { return _importPatternsCommand ?? (_importPatternsCommand = new RelayCommand(ImportPatterns)); }
        }

        public RelayCommand ProceedToPatternSelectionCommand
        {
            get { return _proceedToPatternSelectionCommand ?? (_proceedToPatternSelectionCommand = new RelayCommand(ProceedToPatternSelection)); }
        }

        public RelayCommand InstagramSurfCommand
        {
            get { return _instagramSurfCommand ?? (_instagramSurfCommand = new RelayCommand(InstagramSurf)); }
        }


        public RelayCommand CheckPrintingStatusCommand
        {
            get { return _checkPrintingStatusCommand ?? (_checkPrintingStatusCommand = new RelayCommand(CheckPrintingStatus)); }
        }

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
