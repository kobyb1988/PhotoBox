using System;
using System.Collections.Generic;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.Utils.Services;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class AppSettingsExplorerViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private readonly SchedulerService _schedulerService;
        private readonly ImagePrinter _imagePrinter;
        private RelayCommand _saveSettings;
        private RelayCommand _goBackCommand;
        private TimeSpan _dateStart;
        private TimeSpan _dateEnd;
        private bool _showPrinterOnStartup;
        private string _hashTag;
        private string _printerName;
        private IEnumerable<string> _availablePrinters;

        public AppSettingsExplorerViewModel(
            IViewModelNavigator navigator, 
            SettingsProvider settingsProvider,
            IMappingEngine mappingEngine,
            SchedulerService schedulerService,
            ImagePrinter imagePrinter)
        {
            _navigator = navigator;
            _settingsProvider = settingsProvider;
            _mappingEngine = mappingEngine;
            _schedulerService = schedulerService;
            _imagePrinter = imagePrinter;
        }

        public RelayCommand SaveSettings
        {
            get { return _saveSettings ?? (_saveSettings = new RelayCommand(Save)); }
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        public override void Initialize()
        {
            AppSettingsDto settings = _settingsProvider.GetAppSettings();
            if (settings == null)
            {
                HashTag = string.Empty;
                DateStart = TimeSpan.FromHours(DateTime.Now.Hour);
                DateEnd = TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(5));
                ShowPrinterOnStartup = false;
                return;
            }

            PrinterName = settings.PrinterName;
            HashTag = settings.HashTag;

            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(DateStart);

            DateStart = TimeSpan.FromHours(settings.DateStart.Hour);
            DateEnd = TimeSpan.FromHours(settings.DateEnd.Hour);
            ShowPrinterOnStartup = settings.ShowPrinterOnStartup;
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        public TimeSpan DateStart
        {
            get { return _dateStart; }
            set
            {
                if (_dateStart == value)
                    return;

                _dateStart = value;
                RaisePropertyChanged();
            }
        }

        public string PrinterName
        {
            get { return _printerName; }
            set
            {
                if (_printerName == value)
                    return;

                _printerName = value;
                RaisePropertyChanged();
            }
        }

        public string HashTag
        {
            get { return _hashTag; }
            set
            {
                if (_hashTag == value)
                    return;

                _hashTag = value;
                RaisePropertyChanged();
            }
        }

        public TimeSpan DateEnd
        {
            get { return _dateEnd; }
            set
            {
                if (_dateEnd == value)
                    return;

                _dateEnd = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowPrinterOnStartup
        {
            get { return _showPrinterOnStartup; }
            set
            {
                if (_showPrinterOnStartup == value)
                    return;

                _showPrinterOnStartup = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<string> AvailablePrinters
        {
            get { return _availablePrinters ?? (_availablePrinters = _imagePrinter.GetAvailablePrinters()); }
        }

        private void Save()
        {
            _settingsProvider.SaveAppSettings(_mappingEngine.Map<AppSettingsDto>(this));
            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(DateStart);
            _schedulerService.StartInstagramMonitoring(dt);
        }
    }
}
