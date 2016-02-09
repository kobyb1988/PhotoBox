using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Monads;
using System.Threading.Tasks;
using ImageMaker.Camera;
using ImageMaker.CommonView.Helpers;
using ImageMaker.Entities;
using ImageMaker.PatternProcessing.Dto;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;
using Image = System.Drawing.Image;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class CompositionModelProcessor
    {
        private readonly Template _pattern;
        private readonly ImageProcessor _imageProcessor;
        private readonly ImageUtils _imageUtils;

        public event EventHandler<ImageDto> ImageChanged;
        public event EventHandler<int> TimerElapsed;
        public event EventHandler<int> ImageNumberChanged;
        public event EventHandler<CameraEventBase> CameraErrorEvent;

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

            _initialized = true;
        }

        private void ImageProcessorOnCameraErrorEvent(object sender, CameraEventBase cameraError)
        {
            var handler = CameraErrorEvent;
            if (handler != null)
                handler(this, cameraError);
        }

        public virtual async Task<CompositionProcessingResult> TakePicture(byte[] liveViewImageStream,
            AEMode selectedAeMode, ApertureValue selectedAvValue, CameraISOSensitivity selectedIsoSensitivity, ShutterSpeed selectedShutterSpeed, WhiteBalance selectedWhiteBalance)
        {
            Size liveViewImageStreamSize;
            using (var stream = new MemoryStream(liveViewImageStream))
            {
                var img = Image.FromStream(stream);
                liveViewImageStreamSize = img.Size;
            }

            return new CompositionProcessingResult(_pattern, await TakePictureInternal(liveViewImageStreamSize, selectedAeMode, selectedAvValue, selectedIsoSensitivity, selectedShutterSpeed, selectedWhiteBalance));
        }

        protected async Task<byte[]> TakePictureInternal(Size liveViewImageStreamSize, AEMode selectedAeMode, ApertureValue selectedAvValue, CameraISOSensitivity selectedIsoSensitivity, ShutterSpeed selectedShutterSpeed, WhiteBalance selectedWhiteBalance)
        {
            return await Task.Run(() => Run(liveViewImageStreamSize, selectedAeMode, selectedAvValue, selectedIsoSensitivity, selectedShutterSpeed, selectedWhiteBalance));
        }

        private async Task<byte[]> Run(Size liveViewImageStreamSize, AEMode selectedAeMode, ApertureValue selectedAvValue,
            CameraISOSensitivity selectedIsoSensitivity, ShutterSpeed selectedShutterSpeed, WhiteBalance selectedWhiteBalance)
        {
            //CameraSettingsDto settings = new CameraSettingsDto()
            //{
            //    SelectedAeMode = AEMode.Manual,
            //    SelectedAvValue = ApertureValue.AV_8,
            //    SelectedIsoSensitivity = CameraISOSensitivity.ISO_400,
            //    SelectedWhiteBalance = WhiteBalance.Daylight,
            //    SelectedShutterSpeed = ShutterSpeed.TV_200
            //};

            var settings=GetCameraPhotoSettings();
            
            List<byte[]> pictures = new List<byte[]>();

            for (int i = 0; i < _pattern.Images.Count; i++)
            {
                RaiseImageNumberChanged(i + 1);
                RaiseTimerElapsed(4);
                await Task.Delay(TimeSpan.FromSeconds(2));

                for (int j = 3; j >= 0; j--)
                {
                    RaiseTimerElapsed(j);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                SetCameraSettings(Enum.Parse(typeof(AEMode), settings.SelectedAeMode),
                    Enum.Parse(typeof(WhiteBalance), settings.SelectedWhiteBalance),
                    Enum.Parse(typeof(ApertureValue), settings.SelectedAvValue),
                    Enum.Parse(typeof(CameraISOSensitivity), settings.SelectedIsoSensitivity),
                    Enum.Parse(typeof(ShutterSpeed), settings.SelectedShutterSpeed));
                //await Task.Delay(TimeSpan.FromSeconds(1));

                //RaiseImageNumberChanged(i + 1);
                await Task.Delay(TimeSpan.FromSeconds(1));
                try
                {
                    byte[] picture = await _imageProcessor.DoTakePicture();
                    pictures.Add(picture);
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(TimeSpan.FromSeconds(3)); //todo

                SetCameraSettings(selectedAeMode, selectedWhiteBalance,
                    selectedAvValue, selectedIsoSensitivity,
                    selectedShutterSpeed);
                StopLiveView();
                StartLiveView();
            }

            byte[] result = _imageUtils.ProcessImages(pictures, liveViewImageStreamSize, _pattern);
            return result;
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

        private dynamic GetCameraPhotoSettings()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(baseDir, "CameraPhotoSettings.txt");
            if (!File.Exists(filePath)) return null;

            var lines = File.ReadAllLines(Path.Combine(baseDir, "CameraPhotoSettings.txt"));
            return new
            {
                SelectedAeMode= lines.SingleOrDefault(x => x.Contains("SelectedAeMode")).With(x => x.Replace("SelectedAeMode=", "")),
                SelectedAvValue = lines.SingleOrDefault(x => x.Contains("SelectedAvValue")).With(x => x.Replace("SelectedAvValue=", "")),
                SelectedIsoSensitivity = lines.SingleOrDefault(x => x.Contains("SelectedIsoSensitivity")).With(x => x.Replace("SelectedIsoSensitivity=", "")),
                SelectedWhiteBalance = lines.SingleOrDefault(x => x.Contains("SelectedWhiteBalance")).With(x => x.Replace("SelectedWhiteBalance=", "")),
                SelectedShutterSpeed = lines.SingleOrDefault(x => x.Contains("SelectedShutterSpeed")).With(x => x.Replace("SelectedShutterSpeed=", ""))
            };
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
    }
}
