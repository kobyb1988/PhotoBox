using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Monads;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using ImageMaker.AdminViewModels.Services;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.Utils.Services;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class CurrentSessionViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SessionService _sessionService;
        private readonly ImagePrinter _printer;
        private RelayCommand _goBackCommand;
        private ObservableCollection<CheckableImageWrapper> _images;
        private bool _isBusyLoading;
        private RelayCommand _completeSessionCommand;
        private RelayCommand _showImagesFolderCommand;
        private bool _isSessionActive;

        private string _lastSessionFolderPath;
        private RelayCommand _printCommand;
        private RelayCommand<CheckableImageWrapper> _checkCommand;

        private string _printerName;

        public CurrentSessionViewModel(
            IViewModelNavigator navigator,
            SessionService sessionService,
            ImagePrinter printer,
            SettingsProvider settings
            )
        {
            _navigator = navigator;
            _sessionService = sessionService;
            _printer = printer;
            AppSettingsDto appSettings = settings.GetAppSettings();
            if (appSettings != null)
                _printerName = appSettings.PrinterName;
        }

        public override void Initialize()
        {
            IsBusyLoading = true;
            IsSessionActive = _sessionService.GetIsSessionActive();
            _lastSessionFolderPath = _sessionService.GetLastSessionFolderPath();
            _sessionService.GetImagesAsync()
                .ContinueWith(t =>
                              {
                                  t.Result.Do(x => x.CopyTo(Images));
                                  IsBusyLoading = false;
                              }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public bool IsSessionActive
        {
            get { return _isSessionActive; }
            set
            {
                if (_isSessionActive == value)
                    return;

                _isSessionActive = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        public RelayCommand CompleteSessionCommand
        {
            get { return _completeSessionCommand 
                ?? (_completeSessionCommand = new RelayCommand(CompleteSession, () => IsSessionActive)); }
        }

        public RelayCommand ShowImagesFolderCommand
        {
            get { return _showImagesFolderCommand ?? 
                (_showImagesFolderCommand = new RelayCommand(ShowInFolder, () => !string.IsNullOrEmpty(_lastSessionFolderPath))); }
        }

        public RelayCommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new RelayCommand(Print, () => _checkedImages.IsValueCreated && _checkedImages.Value.Count > 0)); }
        }

        public RelayCommand<CheckableImageWrapper> CheckCommand
        {
            get { return _checkCommand ?? (_checkCommand = new RelayCommand<CheckableImageWrapper>(Check)); }
        }

        private readonly Lazy<List<CheckableImageWrapper>> _checkedImages = new Lazy<List<CheckableImageWrapper>>();
 
        private void Check(CheckableImageWrapper image)
        {
            if (_checkedImages.Value.Contains(image))
            {
                _checkedImages.Value.Remove(image);
                return;
            }

            _checkedImages.Value.Add(image);
            PrintCommand.RaiseCanExecuteChanged();
        }

        private void Print()
        {
            Action<byte[]> print = null;
            if (!string.IsNullOrEmpty(_printerName))
                print = (data) => _printer.Print(data, _printerName);
            else
            {
                print = (data) => _printer.Print(data);
            }

            foreach (var image in _checkedImages.Value)
            {
                print(image.Image.Data);
            }
        }

        private void ShowInFolder()
        {
            if (!Directory.Exists(_lastSessionFolderPath))
                return;

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = _lastSessionFolderPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void CompleteSession()
        {
            _sessionService.StopSession();
            IsSessionActive = false;
            CompleteSessionCommand.RaiseCanExecuteChanged();
        }

        public ObservableCollection<CheckableImageWrapper> Images
        {
            get { return _images ?? (_images = new ObservableCollection<CheckableImageWrapper>()); }
        }

        public bool IsBusyLoading
        {
            get { return _isBusyLoading; }
            set
            {
                if (_isBusyLoading == value)
                    return;

                _isBusyLoading = value;
                RaisePropertyChanged();
            }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }
    }
}
