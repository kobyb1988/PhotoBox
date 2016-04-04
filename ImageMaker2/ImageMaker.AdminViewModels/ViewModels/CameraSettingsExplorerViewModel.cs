using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Async;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.PatternProcessing;
using ImageMaker.PatternProcessing.Dto;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.SDKData;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class CameraSettingsExplorerViewModel : BaseViewModel
    {
        #region Fields and Properties
        static AutoResetEvent _cameraStreamSynchronize;
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private RelayCommand _saveSettings;
        private RelayCommand _goBackCommand;
        private RelayCommand _takePhotoCommand;
        #region CameraSettings
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
        private ExposureCompensation _selectedPhotoCompensation;
        private WhiteBalance _selectedPhotoWhiteBalance;
        private CameraISOSensitivity _selectedPhotoIsoSensitivity;
        private ShutterSpeed _selectedPhotoShutterSpeed;
        private ApertureValue _selectedPhotoAvValue;
        private IList<KeyValuePair<AEMode, string>> _aeModeValues;
        private AEMode _selectedAeMode;
        private AEMode _selectedPhotoAeMode;
        #endregion
        private bool _previewReady;
        private byte[] _liveViewImageStream;
        private readonly CompositionModelProcessor _imageProcessor;
        private int _width;
        private int _height;
        private bool _sessionOpened;

        #region CameraSettins Properties

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
                RaisePropertyChanged();
            }
        }

        #endregion

        public byte[] LiveViewImageStream
        {
            get { return _liveViewImageStream; }
            set
            {
                _liveViewImageStream = value;
                RaisePropertyChanged();
            }
        }

        public bool PreviewReady
        {
            get { return _previewReady; }
            set { Set(() => PreviewReady, ref _previewReady, value); }
        }

        public int Width
        {
            get { return _width; }
            set { Set(() => Width, ref _width, value); }
        }

        public int Height
        {
            get { return _height; }
            set { Set(() => Height, ref _height, value); }
        }
        #endregion


        public CameraSettingsExplorerViewModel(
            IViewModelNavigator navigator,
            SettingsProvider settingsProvider,
            CompositionModelProcessor imageProcessor,
            IMappingEngine mappingEngine)
        {
            _navigator = navigator;
            _settingsProvider = settingsProvider;
            _imageProcessor = imageProcessor;
            _mappingEngine = mappingEngine;
        }

        #region Methods
        public override void Initialize()
        {
            _imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged += ImageProcessorOnStreamChanged;
            _imageProcessor.CameraAddEvent += ImageProcessorCameraAddEvent;
            _imageProcessor.CameraRemoveEvent += ImageProcessorCameraRemoveEvent;
            _imageProcessor.InitializeProcessor();
            OpenSession();
            if (!_sessionOpened)
                return;

            _cameraStreamSynchronize = new AutoResetEvent(false);
            StartLiveView();

        }

        private void StartLiveView()
        {
            _imageProcessor.StartLiveView();
            SetupRealPropertiesFromCamera();

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

        private void CloseSession()
        {
            _imageProcessor.CloseSession();
            _sessionOpened = false;
        }

        private void OpenSession()
        {
            var result = _imageProcessor.OpenSession();
            if (!result)
            {
                PreviewReady = false;
                _sessionOpened = false;
            }
            else
            {
                PreviewReady = true;
                _sessionOpened = true;
            }
        }
        private void GoBack()
        {
            Dispose();
            _cameraStreamSynchronize.Do(x => x.Set());
            _navigator.NavigateBack(this);
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

        private void Save()
        {
            _settingsProvider.SaveCameraSettings(_mappingEngine.Map<CameraSettingsDto>(this));
        }

        private async Task<byte[]> TakePhoto()
        {
            try
            {
                _cameraStreamSynchronize.WaitOne();
                var copyLiveViewStream = LiveViewImageStream;

                var stream = await _imageProcessor.TakeTestPictureAsync(copyLiveViewStream,
                    _mappingEngine.Map<CameraSettingsDto>(this));

                LiveViewImageStream = stream;
                await Task.Delay(5000);

                SetCameraSettings(SelectedAeMode, SelectedWhiteBalance,
                SelectedAvValue, SelectedIsoSensitivity,
                SelectedShutterSpeed);

                StartLiveView();

                return stream;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        private void SetCameraSettings(AEMode aeMode, WhiteBalance balance, ApertureValue apertureValue,
            CameraISOSensitivity cameraIsoSensitivity, ShutterSpeed shutterSpeed)
        {
            _imageProcessor.SetSetting((uint)PropertyId.AEMode, (uint)aeMode);
            _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)balance);
            _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)apertureValue);
            //_imageProcessor.SetSetting((uint)PropertyId.ExposureCompensation, (uint)));
            _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)cameraIsoSensitivity);
            _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)shutterSpeed);
        }
        #region ImageProcessor Handling
        private void ImageProcessorOnCameraErrorEvent(object sender, CameraEventBase cameraErrorInfo)
        {
            switch (cameraErrorInfo.EventType)
            {
                case CameraEventType.Shutdown:
                    // Dispose();

                    break;
                case CameraEventType.Error:

                    var ev = cameraErrorInfo as ErrorEvent;
                    if (ev != null && ev.ErrorCode == ReturnValue.TakePictureAutoFocusNG)
                    {
                        //_dialogService.ShowInfo("Не удалось сфокусироваться. Пожалуйста, повторите попытку.");
                        Dispose();
                        Initialize();
                    }
                    if (ev != null && ev.ErrorCode == ReturnValue.NotSupported)
                        return;
                    break;
            }
        }
        private void ImageProcessorOnStreamChanged(object sender, ImageDto image)
        {
            Width = image.Width;
            Height = image.Height;
            if (image.ImageData.Length > 0)
            {
                LiveViewImageStream = image.ImageData;
                _cameraStreamSynchronize.Set();
            }
        }

        private void ImageProcessorCameraRemoveEvent(object sender, EventArgs e)
        {
            //Пытаемся открыть сессию если камера норм то показываем превью
            //_imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            //_imageProcessor.ImageChanged += ImageProcessorOnStreamChanged;
            //_imageProcessor.CameraAddEvent += ImageProcessorCameraAddEvent;
            //_imageProcessor.CameraRemoveEvent += ImageProcessorCameraRemoveEvent;
            //_imageProcessor.InitializeProcessor();
            //CloseSession();//TODO Разобраться почему при включении камеры не приходит событие
            Dispose();
            _imageProcessor.SubscribeToCameraAddEvent();
            // Initialize();
            //    _imageProcessor.Dispose();
            PreviewReady = false;
        }

        private void ImageProcessorCameraAddEvent(object sender, EventArgs e)
        {
            //Пытаемся открыть сессию если камера норм то показываем превью
            _imageProcessor.InitializeProcessor();
            OpenSession();
            if (!_sessionOpened)
                return;

            StartLiveView();
            // Initialize();
        }

        public override void Dispose()
        {
            _imageProcessor.CameraErrorEvent -= ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged -= ImageProcessorOnStreamChanged;
            _imageProcessor.CameraAddEvent -= ImageProcessorCameraAddEvent;
            _imageProcessor.CameraRemoveEvent -= ImageProcessorCameraRemoveEvent;
            _sessionOpened = false;
            PreviewReady = false;
            _imageProcessor.Dispose();
        }
        #endregion

        #endregion

        #region Commands

        public RelayCommand SaveSettings => _saveSettings ?? (_saveSettings = new RelayCommand(Save));

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public RelayCommand TakePhotoCommand
            => _takePhotoCommand ?? (_takePhotoCommand = new RelayCommand(async () => await TakePhoto()));

        #endregion
    }
}
