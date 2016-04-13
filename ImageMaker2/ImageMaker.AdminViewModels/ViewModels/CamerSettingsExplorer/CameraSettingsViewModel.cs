using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.SDKData.Enums;

namespace ImageMaker.AdminViewModels.ViewModels.CamerSettingsExplorer
{
    public class CameraSettingsViewModel : BaseViewModel, ICopyable<CameraSettingsViewModel>
    {
        internal static Lazy<StackStorage> Stack = new Lazy<StackStorage>();

        private readonly CompositionModelProcessor _imageProcessor;
        private readonly SettingsProvider _settingsProvider;
        private ExposureCompensation _selectedCompensation;
        private WhiteBalance _selectedWhiteBalance;
        private CameraISOSensitivity _selectedIsoSensitivity;
        private ShutterSpeed _selectedShutterSpeed;
        private ApertureValue _selectedAvValue;
        private AEMode _selectedAeMode;
        private ExposureCompensation _selectedPhotoCompensation;
        private WhiteBalance _selectedPhotoWhiteBalance;
        private CameraISOSensitivity _selectedPhotoIsoSensitivity;
        private ShutterSpeed _selectedPhotoShutterSpeed;
        private ApertureValue _selectedPhotoAvValue;
        private AEMode _selectedPhotoAeMode;

        private IList<KeyValuePair<ShutterSpeed, string>> _shutterSpeedValues;
        private IList<KeyValuePair<ApertureValue, string>> _apertureValues;
        private IList<KeyValuePair<WhiteBalance, string>> _whiteBalanceValues;
        private IList<KeyValuePair<CameraISOSensitivity, string>> _isoValues;
        private IList<KeyValuePair<ExposureCompensation, string>> _exposureValues;

        private IList<KeyValuePair<AEMode, string>> _aeModeValues;


        public IList<KeyValuePair<ShutterSpeed, string>> ShutterSpeedValues
           => _shutterSpeedValues ?? (_shutterSpeedValues = GetEnumValues<ShutterSpeed>());

        public IList<KeyValuePair<ApertureValue, string>> AvValues
            => _apertureValues ?? (_apertureValues = GetEnumValues<ApertureValue>());

        public IList<KeyValuePair<AEMode, string>> AEModeValues
            => _aeModeValues ?? (_aeModeValues = GetEnumValues<AEMode>());

        public IList<KeyValuePair<WhiteBalance, string>> WhiteBalanceValues
            => _whiteBalanceValues ?? (_whiteBalanceValues = GetEnumValues<WhiteBalance>());

        public IList<KeyValuePair<CameraISOSensitivity, string>> ISOValues
            => _isoValues ?? (_isoValues = GetEnumValues<CameraISOSensitivity>());

        public IList<KeyValuePair<ExposureCompensation, string>> ExposureValues
            => _exposureValues ?? (_exposureValues = GetEnumValues<ExposureCompensation>());


        public ExposureCompensation SelectedCompensation
        {
            get { return _selectedCompensation; }
            set
            {
                if (_selectedCompensation == value)
                    return;

                _selectedCompensation = value;

                PushState();
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
                _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)_selectedWhiteBalance);
                PushState();
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

                _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)_selectedIsoSensitivity);
                PushState();
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

                _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)_selectedShutterSpeed);
                _selectedShutterSpeed = value;
                PushState();
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
                _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)_selectedAvValue);
                PushState();
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
                _imageProcessor.SetSetting((uint)PropertyId.AEMode, (uint)_selectedAeMode);
                PushState();
                RaisePropertyChanged();
            }
        }


        public ExposureCompensation SelectedPhotoCompensation
        {
            get { return _selectedPhotoCompensation; }
            set
            {
                if (_selectedPhotoCompensation == value)
                    return;

                _selectedPhotoCompensation = value;
                PushState();
                RaisePropertyChanged();
            }
        }

        public WhiteBalance SelectedPhotoWhiteBalance
        {
            get { return _selectedPhotoWhiteBalance; }
            set
            {
                if (_selectedPhotoWhiteBalance == value)
                    return;

                _selectedPhotoWhiteBalance = value;
                PushState();
                RaisePropertyChanged();
            }
        }

        public CameraISOSensitivity SelectedPhotoIsoSensitivity
        {
            get { return _selectedPhotoIsoSensitivity; }
            set
            {
                if (_selectedPhotoIsoSensitivity == value)
                    return;

                _selectedPhotoIsoSensitivity = value;
                PushState();
                RaisePropertyChanged();
            }
        }

        public ShutterSpeed SelectedPhotoShutterSpeed
        {
            get { return _selectedPhotoShutterSpeed; }
            set
            {
                if (_selectedPhotoShutterSpeed == value)
                    return;

                _selectedPhotoShutterSpeed = value;
                PushState();
                RaisePropertyChanged();
            }
        }

        public ApertureValue SelectedPhotoAvValue
        {
            get { return _selectedPhotoAvValue; }
            set
            {
                if (_selectedPhotoAvValue == value)
                    return;
                _selectedPhotoAvValue = value;
                PushState();
                RaisePropertyChanged();
            }
        }

        public AEMode SelectedPhotoAeMode
        {
            get { return _selectedPhotoAeMode; }
            set
            {
                if (_selectedPhotoAeMode == value)
                    return;

                _selectedPhotoAeMode = value;
                PushState();
                RaisePropertyChanged();
            }
        }

        public bool CanRedoChanges => Stack.IsValueCreated && Stack.Value.CanRedo;
        public bool CanSave => Stack.IsValueCreated && Stack.Value.CanUndo;
        public bool CanUndo => Stack.IsValueCreated && Stack.Value.CanUndo;


        public CameraSettingsViewModel(CompositionModelProcessor imageProcessor, SettingsProvider settingsProvider)
        {
            _imageProcessor = imageProcessor;
            _settingsProvider = settingsProvider;
        }

        public CameraSettingsViewModel Copy()
        {
            return new CameraSettingsViewModel(_imageProcessor, _settingsProvider)
            {
                _selectedAeMode = SelectedAeMode,
                _selectedAvValue = SelectedAvValue,
                _selectedShutterSpeed = SelectedShutterSpeed,
                _selectedCompensation = SelectedCompensation,
                _selectedIsoSensitivity = SelectedIsoSensitivity,
                _selectedWhiteBalance = SelectedWhiteBalance,

                _selectedPhotoAeMode = SelectedPhotoAeMode,
                _selectedPhotoAvValue = SelectedPhotoAvValue,
                _selectedPhotoCompensation = SelectedPhotoCompensation,
                _selectedPhotoIsoSensitivity = SelectedPhotoIsoSensitivity,
                _selectedPhotoShutterSpeed = SelectedPhotoShutterSpeed,
                _selectedPhotoWhiteBalance = SelectedPhotoWhiteBalance,
            };
        }

        public void CopyTo(CameraSettingsViewModel to)
        {
            to._selectedAeMode = SelectedAeMode;
            to._selectedAvValue = SelectedAvValue;
            to._selectedShutterSpeed = SelectedShutterSpeed;
            to._selectedCompensation = SelectedCompensation;
            to._selectedIsoSensitivity = SelectedIsoSensitivity;
            to._selectedWhiteBalance = SelectedWhiteBalance;

            to._selectedPhotoAeMode = SelectedPhotoAeMode;
            to._selectedPhotoAvValue = SelectedPhotoAvValue;
            to._selectedPhotoCompensation = SelectedPhotoCompensation;
            to._selectedPhotoIsoSensitivity = SelectedPhotoIsoSensitivity;
            to._selectedPhotoShutterSpeed = SelectedPhotoShutterSpeed;
            to._selectedPhotoWhiteBalance = SelectedPhotoWhiteBalance;
        }

        /// <summary>
        /// Устанавливает значения для свойств, которые возвращает API камеры. Если API возвращает пустой список, то значения свойств останутся просто списком всех доступных значений
        /// </summary>
        private void SetupRealPropertiesFromCamera()
        {
            var iso = _imageProcessor.GetSupportedEnumProperties<CameraISOSensitivity>(PropertyId.ISOSpeed).ToList().ToKeyValue();
            if (iso.Any())
                _isoValues = iso;

            var shutterSpeed = _imageProcessor.GetSupportedEnumProperties<ShutterSpeed>(PropertyId.Tv).ToList().ToKeyValue();
            if (shutterSpeed.Any())
                _shutterSpeedValues = shutterSpeed;

            var aperture = _imageProcessor.GetSupportedEnumProperties<ApertureValue>(PropertyId.Av).ToList().ToKeyValue();
            if (aperture.Any())
                _apertureValues = aperture;

            var aeMode = _imageProcessor.GetSupportedEnumProperties<AEMode>(PropertyId.AEMode).ToList().ToKeyValue();
            if (aeMode.Any())
                _aeModeValues = aeMode;

            //var whiteBalance = _imageProcessor.GetSupportedEnumProperties<WhiteBalance>(PropertyId.WhiteBalance).ToList().ToKeyValue();
            //if (whiteBalance.Any())
            //    _whiteBalanceValues = whiteBalance;

            var exposure = _imageProcessor.GetSupportedEnumProperties<ExposureCompensation>(PropertyId.ExposureCompensation).ToList().ToKeyValue();
            if (exposure.Any())
                _exposureValues = exposure;
        }

        private void SetupSavedCameraSettings()
        {
            var settings = _settingsProvider.GetCameraSettings();
            if (settings == null)
            {
                SelectedAeMode = AEModeValues.First().Key;
                SelectedAvValue = AvValues.First().Key;
                SelectedCompensation = ExposureValues.First().Key;
                SelectedIsoSensitivity = ISOValues.First().Key;
                SelectedShutterSpeed = ShutterSpeedValues.First().Key;
                SelectedWhiteBalance = WhiteBalanceValues.First().Key;

                SelectedPhotoAeMode = AEModeValues.First().Key;
                SelectedPhotoAvValue = AvValues.First().Key;
                SelectedPhotoCompensation = ExposureValues.First().Key;
                SelectedPhotoIsoSensitivity = ISOValues.First().Key;
                SelectedPhotoShutterSpeed = ShutterSpeedValues.First().Key;
                SelectedPhotoWhiteBalance = WhiteBalanceValues.First().Key;
                return;
            }

            SelectedAeMode = settings.SelectedAeMode;
            SelectedAvValue = settings.SelectedAvValue;
            SelectedCompensation = settings.SelectedCompensation;
            SelectedIsoSensitivity = settings.SelectedIsoSensitivity;
            SelectedShutterSpeed = settings.SelectedShutterSpeed;
            SelectedWhiteBalance = settings.SelectedWhiteBalance;

            SelectedPhotoAeMode = settings.SelectedPhotoAeMode;
            SelectedPhotoAvValue = settings.SelectedPhotoAvValue;
            SelectedPhotoCompensation = settings.SelectedPhotoCompensation;
            SelectedPhotoIsoSensitivity = settings.SelectedPhotoIsoSensitivity;
            SelectedPhotoShutterSpeed = settings.SelectedPhotoShutterSpeed;
            SelectedPhotoWhiteBalance = settings.SelectedPhotoWhiteBalance;

            #region Костыль, если не поменять свойство выдержки на рядом стоящие значения, то смена остальных свойств не будет сразу применяться в LiveView
            SelectedShutterSpeed = settings.SelectedShutterSpeed.GetNextEnumValue();
            SelectedShutterSpeed = settings.SelectedShutterSpeed.GetNextEnumValue();
            SelectedShutterSpeed = settings.SelectedShutterSpeed;
            #endregion
        }

        private IList<KeyValuePair<TEnum, string>> GetEnumValues<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum))
                .OfType<TEnum>()
                .ToDictionary(x => x, x => x.GetDescription())
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Select(x => new KeyValuePair<TEnum, string>(x.Key, x.Value))
                .ToList();
        }

        public void SetCameraSettings()
        {
            _imageProcessor.SetSetting((uint)PropertyId.AEMode, (uint)SelectedAeMode);
            _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)SelectedWhiteBalance);
            _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)SelectedAvValue);
            //_imageProcessor.SetSetting((uint)PropertyId.ExposureCompensation, (uint)));
            _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)SelectedIsoSensitivity);
            _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)SelectedShutterSpeed);
        }

        private void PushState()
        {
            Stack.Value.Do(this);
        }

        public void PrepareToLiveView(bool setupDefaultSettings)
        {
            SetupRealPropertiesFromCamera();

            if (setupDefaultSettings)
                SetupSavedCameraSettings();
        }
        public void ClearChanges()
        {
            if (Stack.IsValueCreated)
                Stack.Value.Clear();
        }

        public void ClearStackChanged() => Stack.Value.Clear();
        public void UndoChanges() => Stack.Value.Undo();
        public void RedoChanges() => Stack.Value.Redo();
        public void ResetChanges() => Stack.Value.Reset();
    }
}
