using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ImageMaker.Camera;
using ImageMaker.Entities;
using ImageMaker.PatternProcessing.Dto;
using ImageMaker.SDKData;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;
using NLog;
using Image = System.Drawing.Image;
using Size = System.Drawing.Size;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class CompositionModelProcessor
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Template _pattern;
        private readonly ImageProcessor _imageProcessor;
        private readonly ImageUtils _imageUtils;

        public event EventHandler<ImageDto> ImageChanged;
        public event EventHandler<int> TimerElapsed;
        public event EventHandler<int> ImageNumberChanged;
        public event EventHandler<CameraEventBase> CameraErrorEvent;
        public event EventHandler CameraRemoveEvent;
        public event EventHandler CameraAddEvent;
        public bool CameraExist => _imageProcessor.CameraExist;

        public CompositionModelProcessor(Template pattern, ImageProcessor imageProcessor, ImageUtils imageUtils)
        {
            _pattern = pattern;
            _imageProcessor = imageProcessor;
            _imageUtils = imageUtils;
        }

        private bool _initialized;

        public void InitializeProcessor()
        {
            if (_initialized)
                return;

            _imageProcessor.Initialize();
            _imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            _imageProcessor.AddCamera += OnAddCamera;
            _imageProcessor.RemoveCamera += OnRemoveCamera;

            _initialized = true;
        }

        public void SubscribeToCameraAddEvent()
        {
            // _imageProcessor.AddCamera += OnAddCamera;
            _imageProcessor.SubscribeToCameraAddEvent();
        }

        private void ImageProcessorOnCameraErrorEvent(object sender, CameraEventBase cameraError)
        {
            CameraErrorEvent?.Invoke(this, cameraError);
        }

        public async Task<byte[]> TakeTestPictureAsync(byte[] liveViewImageStream, CameraSettingsDto settings)
        {
            SetCameraSettings(settings.SelectedAeMode, settings.SelectedPhotoWhiteBalance,
             settings.SelectedPhotoAvValue, settings.SelectedPhotoIsoSensitivity,
             settings.SelectedPhotoShutterSpeed);

            byte[] picture = await Task.Run(async () =>
            {
                //    await Task.Delay(500);//TODO Костыль, не понятно почему, но камере реально необходимо время, что бы свойства установились
                var res = await _imageProcessor.DoTakePictureAsync();
                return res;
            });
            StopLiveView();
            return picture;
        }

        public virtual async Task<CompositionProcessingResult> TakePictureAsync(Size liveViewImageStreamSize, CameraSettingsDto settings,
            CancellationToken token)
        {
            return new CompositionProcessingResult(_pattern, await TakePictureInternal(liveViewImageStreamSize, settings, token));
        }

        protected async Task<byte[]> TakePictureInternal(Size liveViewImageStreamSize, CameraSettingsDto settings, CancellationToken token)
        {
            return await Task.Run(() => Run(liveViewImageStreamSize, settings, token), token);
        }

        private async Task<byte[]> Run(Size liveViewImageStreamSize, CameraSettingsDto settings, CancellationToken token)
        {
            var pictures = new List<byte[]>();

            for (var i = 0; i < _pattern.Images.Count; i++)
            {
                _logger.Trace($"Фото №{i}");

                token.ThrowIfCancellationRequested();
                RaiseImageNumberChanged(i + 1);

                await Task.Delay(TimeSpan.FromSeconds(1), token);

                Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);

                for (var j = 5; j >= 0; j--)
                {
                    var j1 = j;
                    RaiseTimerElapsed(j1);
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
                _logger.Trace("Отсчёт закончен. Начало применения настроек для камеры.");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetCameraSettings(settings.SelectedPhotoAeMode, settings.SelectedPhotoWhiteBalance,
                        settings.SelectedPhotoAvValue, settings.SelectedPhotoIsoSensitivity,
                        settings.SelectedPhotoShutterSpeed);
                    _logger.Trace("Настройки для камеры применены.");
                });

                _logger.Trace("Команда с настройками для фото камере послана.");

                //await Task.Delay(TimeSpan.FromSeconds(1));

                //RaiseImageNumberChanged(i + 1);
                //await Task.Delay(TimeSpan.FromSeconds(1), token);
                token.ThrowIfCancellationRequested();
                byte[] picture = await _imageProcessor.DoTakePictureAsync();
                pictures.Add(picture);

                token.ThrowIfCancellationRequested();
                //await Task.Delay(TimeSpan.FromSeconds(3), token); //todo
                _logger.Trace("Фотография сделана. Начало применения настроек для LiveView");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetCameraSettings(settings.SelectedAeMode, settings.SelectedWhiteBalance,
                        settings.SelectedAvValue, settings.SelectedIsoSensitivity,
                        settings.SelectedShutterSpeed);
                    _logger.Trace("Настройки для камеры применены.");
                });
                _logger.Trace("Команда с настройками для LiveView камере послана.");

                _logger.Trace("Стоп LiveView.");
                StopLiveView();
                _logger.Trace("Старт LiveView.");
                StartLiveView();
            }

            byte[] result = _imageUtils.ProcessImages(pictures, liveViewImageStreamSize, _pattern);
            return result;
        }

        private void SetCameraSettings(AEMode aeMode, WhiteBalance balance, ApertureValue apertureValue,
            CameraISOSensitivity cameraIsoSensitivity, ShutterSpeed shutterSpeed)
        {
            //_imageProcessor.SetSetting((uint)PropertyId.AEMode, (uint)aeMode); TODO Не поддерживается Eos 1100
            _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)balance);
            _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)apertureValue);
            //_imageProcessor.SetSetting((uint)PropertyId.ExposureCompensation, (uint)));
            _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)cameraIsoSensitivity);
            _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)shutterSpeed);
        }

        private void OnRemoveCamera(object sender, EventArgs e)
        {
            CameraRemoveEvent?.Invoke(sender, e);
        }

        private void OnAddCamera(object sender, EventArgs e)
        {
            CameraAddEvent?.Invoke(sender, e);
        }

        private void RaiseImageNumberChanged(int newNumber)
        {
            ImageNumberChanged?.Invoke(this, newNumber);
        }


        private void RaiseTimerElapsed(int tick)
        {
            TimerElapsed?.Invoke(this, tick);
        }

        private void RaiseImageChanged(byte[] imgBuf)
        {
            ImageChanged?.Invoke(this, new ImageDto(imgBuf));
        }

        //private void RaiseImageChanged(byte[] imgBuf, int width, int height)
        //{
        //    ImageChanged?.Invoke(this, new ImageDto(imgBuf, width, height));
        //}

        private void ImageProcessorOnStreamChanged(object sender, byte[] bytes)
        {
            RaiseImageChanged(bytes);
        }

        public void Dispose()
        {
            _initialized = false;
            _imageProcessor.CameraErrorEvent -= ImageProcessorOnCameraErrorEvent;
            _imageProcessor.RemoveCamera -= OnRemoveCamera;
            _imageProcessor.AddCamera -= OnAddCamera;

            _imageProcessor.Dispose();
        }

        public virtual void SetSetting(uint settingId, uint settingValue)
        {
            _imageProcessor.SetSetting(settingId, settingValue);
        }

        public virtual void SetFocus(uint focus)
        {
            _imageProcessor.SetFocus(focus);
        }

        public virtual void StopLiveView()
        {
            _imageProcessor.StreamChanged -= ImageProcessorOnStreamChanged;
            //todo stop live view
            //_imageProcessor.StartLiveView();
        }

        public virtual void StartLiveView()
        {
            _imageProcessor.StreamChanged += ImageProcessorOnStreamChanged;
            _imageProcessor.StartLiveView();
        }

        public virtual void RefreshCamera()
        {
            _imageProcessor.DoRefresh();
        }

        public virtual void CloseSession()
        {
            _imageProcessor.CloseSession();
        }

        public virtual bool OpenSession()
        {
            InitializeProcessor();

            return _imageProcessor.DoOpenSession();
        }

        public IEnumerable<TEnumType> GetSupportedEnumProperties<TEnumType>(PropertyId propertyId)
            where TEnumType : struct, IConvertible
        {
            var uintProperties = _imageProcessor.GetSettingList(propertyId).Cast<TEnumType>();
            return uintProperties;
        }
    }
}
