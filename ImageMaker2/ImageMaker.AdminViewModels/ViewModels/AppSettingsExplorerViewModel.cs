using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Dto;
using ImageMaker.Common.Enums;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.Themes.CustomControls;
using ImageMaker.Utils.Services;
using System.ComponentModel;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class AppSettingsExplorerViewModel : BaseViewModel
    {
        #region Properties And Settings
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private readonly SchedulerService _schedulerService;
        private readonly ImagePrinter _imagePrinter;
        private RelayCommand _saveSettings;
        private RelayCommand _goBackCommand;
        private RelayCommand _changeUpCommand;
        private RelayCommand _changeDownCommand;
        private Hour _dateStart;
        private Hour _dateEnd;
        private bool _showPrinterOnStartup;
        private string _hashTag;
        private string _printerName;
        private IEnumerable<string> _availablePrinters;
        private byte _maxPrinterCopies;
        private readonly byte _maxAvailableCopies;
        private bool _instaPrinterVisible;
        private bool _selfyPrinterVisible;
        private bool _isSaveEnable;

        public bool IsSaveEnable
        {
            get
            {
                return _isSaveEnable;
            }
            set
            {
                _isSaveEnable = value;
                SaveSettings.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => IsSaveEnable);
            }
        }
        public byte MaxPrinterCopies
        {
            get { return _maxPrinterCopies; }
            set
            {
                if (_maxPrinterCopies == value)
                    return;
                _maxPrinterCopies = value;
                IsSaveEnable = true;
                RaisePropertyChanged(() => MaxPrinterCopies);
            }
        }


        public Hour DateStart
        {
            get { return _dateStart; }
            set
            {
                if (_dateStart == value)
                    return;
                IsSaveEnable = true;
                _dateStart = value;
                RaisePropertyChanged();
            }
        }

        public bool InstaPrinterVisible
        {
            get { return _instaPrinterVisible; }
            set { Set(() => InstaPrinterVisible, ref _instaPrinterVisible, value); }
        }

        public bool SelfyPrinterVisible
        {
            get { return _selfyPrinterVisible; }
            set { Set(() => SelfyPrinterVisible, ref _selfyPrinterVisible, value); }
        }

        //public Hour DateStart
        //{
        //    get { return new Hour(_dateStart); }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            _dateStart = TimeSpan.Zero;
        //            return;
        //        }
        //        //if (_dateStart == value)
        //        //    return;

        //        _dateStart = value.GetCurrentTime();
        //        RaisePropertyChanged();
        //    }
        //}

        public string PrinterName
        {
            get { return _printerName; }
            set
            {
                if (_printerName == value)
                    return;
                IsSaveEnable = true;
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
                IsSaveEnable = true;
                _hashTag = value;
                RaisePropertyChanged();
            }
        }

        public Hour DateEnd
        {
            get { return _dateEnd; }
            set
            {
                if (_dateEnd == value)
                    return;
                IsSaveEnable = true;
                _dateEnd = value;
                RaisePropertyChanged();
            }
        }

        //public Hour DateEnd
        //{
        //    get { return new Hour(_dateEnd);; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            _dateEnd = TimeSpan.Zero;
        //            return;
        //        }

        //        _dateEnd = value.GetCurrentTime();
        //        RaisePropertyChanged();
        //    }
        //}
        #endregion
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
            _maxAvailableCopies = 255;
        }

        public override void Initialize()
        {
            ModuleSettingDto moduleSetting = _settingsProvider.GetAvailableModules();
            if (moduleSetting != null)
            {
                InstaPrinterVisible = moduleSetting.AvailableModules.Any(x => x == AppModules.InstaPrinter);
                SelfyPrinterVisible = moduleSetting.AvailableModules.Any(x => x != AppModules.InstaPrinter);
            }

            AppSettingsDto settings = _settingsProvider.GetAppSettings();
            if (settings == null)
            {
                HashTag = string.Empty;
                _dateStart = new Hour(TimeSpan.FromHours(DateTime.Now.Hour));
                _dateEnd = new Hour(TimeSpan.FromHours(DateTime.Now.Hour).Add(TimeSpan.FromMinutes(5)));

                RaisePropertyChanged(() => DateStart);
                RaisePropertyChanged(() => DateEnd);

                ShowPrinterOnStartup = false;

                return;
            }

            _printerName = settings.PrinterName;
            _hashTag = settings.HashTag;
            _maxPrinterCopies = settings.MaxPrinterCopies;
            _showPrinterOnStartup = settings.ShowPrinterOnStartup;
            // var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(_dateStart.GetCurrentTime());

            _dateStart = new Hour(TimeSpan.FromHours(settings.DateStart.Hour).Add(TimeSpan.FromMinutes(settings.DateStart.Minute)));
            _dateEnd = new Hour(TimeSpan.FromHours(settings.DateEnd.Hour).Add(TimeSpan.FromMinutes(settings.DateEnd.Minute)));
            /*Из-за того что само время меняется в другом классе*/
            _dateStart.PropertyChanged += OnTimePropertyChange;
            _dateEnd.PropertyChanged += OnTimePropertyChange;
            RaisePropertyChanged(() => DateStart);
            RaisePropertyChanged(() => DateEnd);
            RaisePropertyChanged(() => PrinterName);
            RaisePropertyChanged(() => HashTag);
            RaisePropertyChanged(() => MaxPrinterCopies);
            RaisePropertyChanged(() => ShowPrinterOnStartup);
        }

        private void OnTimePropertyChange(object sender, PropertyChangedEventArgs e)
        {
            IsSaveEnable = true;
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }



        public bool ShowPrinterOnStartup
        {
            get { return _showPrinterOnStartup; }
            set
            {
                if (_showPrinterOnStartup == value)
                    return;
                IsSaveEnable = true;
                _showPrinterOnStartup = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<string> AvailablePrinters => _availablePrinters ?? (_availablePrinters = _imagePrinter.GetAvailablePrinters());

        private void Save()
        {
            _settingsProvider.SaveAppSettings(_mappingEngine.Map<AppSettingsDto>(this));
            var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Add(_dateStart.GetCurrentTime());
            _schedulerService.StartInstagramMonitoring(dt);
            IsSaveEnable = false;
        }

        public RelayCommand SaveSettings => _saveSettings ?? (_saveSettings = new RelayCommand(Save, () => IsSaveEnable));

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public RelayCommand ChangeDownCommand
        {
            get
            {
                return _changeDownCommand ?? (_changeDownCommand = new RelayCommand(() =>
                {
                    MaxPrinterCopies--;
                    ChangeDownCommand.RaiseCanExecuteChanged();
                }, () => MaxPrinterCopies > 1));
            }
        }

        public RelayCommand ChangeUpCommand
        {
            get
            {
                return _changeUpCommand ?? (_changeUpCommand = new RelayCommand(() =>
                {
                    MaxPrinterCopies++;
                    ChangeUpCommand.RaiseCanExecuteChanged();
                }, () => MaxPrinterCopies < _maxAvailableCopies));
            }
        }
    }
}
