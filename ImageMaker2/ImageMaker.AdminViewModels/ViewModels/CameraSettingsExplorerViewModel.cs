using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.SDKData.Enums;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class CameraSettingsExplorerViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private RelayCommand _saveSettings;
        private RelayCommand _goBackCommand;
        private IList<KeyValuePair<ShutterSpeed, string>> _shutterSpeedValues;
        private IList<KeyValuePair<ApertureValue, string>> _apertureValues;
        private IList<KeyValuePair<WhiteBalance, string>> _whiteBalanceValues;
        private IList<KeyValuePair<CameraISOSensitivity, string>> _isoValues;
        private IList<KeyValuePair<ExposureCompensation, string>> _exposureValues;
        private ExposureCompensation _selectedCompensation;
        private WhiteBalance _selectedWhiteBalance;
        private CameraISOSensitivity _selectedIsoSensitivity;
        private ShutterSpeed _selectedShutterSpeed;
        private ApertureValue _selectedAvValue;
        private IList<KeyValuePair<AEMode, string>> _aeModeValues;
        private AEMode _selectedAeMode;

        public CameraSettingsExplorerViewModel(
            IViewModelNavigator navigator, 
            SettingsProvider settingsProvider,
            IMappingEngine mappingEngine)
        {
            _navigator = navigator;
            _settingsProvider = settingsProvider;
            _mappingEngine = mappingEngine;
        }

        public override void Initialize()
        {
            CameraSettingsDto settings = _settingsProvider.GetCameraSettings();
            if (settings == null)
            {
                SelectedAeMode = AEModeValues.First().Key;
                SelectedAvValue = AvValues.First().Key;
                SelectedCompensation = ExposureValues.First().Key;
                SelectedIsoSensitivity = ISOValues.First().Key;
                SelectedShutterSpeed = ShutterSpeedValues.First().Key;
                SelectedWhiteBalance = WhiteBalanceValues.First().Key;
                return;
            }

            SelectedAeMode = settings.SelectedAeMode;
            SelectedAvValue = settings.SelectedAvValue;
            SelectedCompensation = settings.SelectedCompensation;
            SelectedIsoSensitivity = settings.SelectedIsoSensitivity;
            SelectedShutterSpeed = settings.SelectedShutterSpeed;
            SelectedWhiteBalance = settings.SelectedWhiteBalance;
        }

        public RelayCommand SaveSettings
        {
            get { return _saveSettings ?? (_saveSettings = new RelayCommand(Save)); }
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        public IList<KeyValuePair<ShutterSpeed, string>> ShutterSpeedValues
        {
            get
            {
                return _shutterSpeedValues ?? 
                    (_shutterSpeedValues = GetEnumValues<ShutterSpeed>());
            }
        }

        public IList<KeyValuePair<ApertureValue, string>> AvValues
        {
            get
            {
                return _apertureValues ??
                    (_apertureValues = GetEnumValues<ApertureValue>());
            }
        }

        public IList<KeyValuePair<AEMode, string>> AEModeValues
        {
            get { return _aeModeValues ?? (_aeModeValues = GetEnumValues<AEMode>()); }
        }

        public IList<KeyValuePair<WhiteBalance, string>> WhiteBalanceValues
        {
            get { return _whiteBalanceValues ?? (_whiteBalanceValues = GetEnumValues<WhiteBalance>()); }
        }

        public IList<KeyValuePair<CameraISOSensitivity, string>> ISOValues
        {
            get { return _isoValues ?? (_isoValues = GetEnumValues<CameraISOSensitivity>()); }
        }

        public IList<KeyValuePair<ExposureCompensation, string>> ExposureValues
        {
            get { return _exposureValues ?? (_exposureValues = GetEnumValues<ExposureCompensation>()); }
        }

        public ExposureCompensation SelectedCompensation
        {
            get { return _selectedCompensation; }
            set
            {
                if (_selectedCompensation == value)
                    return;

                _selectedCompensation = value;
                RaisePropertyChanged();
            }
        }

        public WhiteBalance SelectedWhiteBalance
        {
            get { return _selectedWhiteBalance; }
            set
            {
                if (_selectedWhiteBalance == value)
                    return;

                _selectedWhiteBalance = value;
                RaisePropertyChanged();
            }
        }

        public CameraISOSensitivity SelectedIsoSensitivity
        {
            get { return _selectedIsoSensitivity; }
            set
            {
                if (_selectedIsoSensitivity == value)
                    return;

                _selectedIsoSensitivity = value;
                RaisePropertyChanged();
            }
        }

        public ShutterSpeed SelectedShutterSpeed
        {
            get { return _selectedShutterSpeed; }
            set
            {
                if (_selectedShutterSpeed == value)
                    return;

                _selectedShutterSpeed = value;
                RaisePropertyChanged();
            }
        }

        public ApertureValue SelectedAvValue
        {
            get { return _selectedAvValue; }
            set
            {
                if (_selectedAvValue == value)
                    return;

                _selectedAvValue = value;
                RaisePropertyChanged();
            }
        }

        public AEMode SelectedAeMode
        {
            get { return _selectedAeMode; }
            set
            {
                if (_selectedAeMode == value)
                    return;

                _selectedAeMode = value;
                RaisePropertyChanged();
            }
        }

        private IList<KeyValuePair<TEnum, string>> GetEnumValues<TEnum>()
        {
            return Enum.GetValues(typeof (TEnum))
                .OfType<TEnum>()
                .ToDictionary(x => x, x => x.GetDescription())
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => new KeyValuePair<TEnum, string>(x.Key, x.Value))
                .ToList();
        } 

        private void Save()
        {
            _settingsProvider.SaveCameraSettings(_mappingEngine.Map<CameraSettingsDto>(this));
        }
    }
}
