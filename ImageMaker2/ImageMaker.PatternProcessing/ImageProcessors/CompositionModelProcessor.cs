using System;
using System.Collections.Generic;
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
using ImageMaker.SDKData.Events;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class CompositionModelProcessor
    {
        private readonly Template _pattern;
        private readonly ImageProcessor _imageProcessor;

        public event EventHandler<ImageDto> ImageChanged;
        public event EventHandler<int> TimerElapsed;
        public event EventHandler<CameraEventBase> CameraErrorEvent;

        public CompositionModelProcessor(Template pattern, ImageProcessor imageProcessor)
        {
            _pattern = pattern;
            _imageProcessor = imageProcessor;
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

        public virtual Task<CompositionProcessingResult> TakePicture()
        {
            TaskCompletionSource<CompositionProcessingResult> source = new TaskCompletionSource<CompositionProcessingResult>();
            
            TakePictureInternal()
                .ContinueWith(t => source.TrySetResult(new CompositionProcessingResult(_pattern, t.Result)));

            return source.Task;
        }

        protected Task<byte[]> TakePictureInternal()
        {
            TaskCompletionSource<byte[]> source = new TaskCompletionSource<byte[]>();
            
            Task.Run(() => Run().Result)
                .ContinueWith(t => source.TrySetResult(t.Result));

            return source.Task;
        }

        private async Task<byte[]> Run()
        {
            List<byte[]> pictures = new List<byte[]>();

            for (int i = 0; i < _pattern.Images.Count; i++)
            {
                RaiseTimerElapsed(4);
                await Task.Delay(TimeSpan.FromSeconds(2));

                for (int j = 3; j >= 0; j--)
                {
                    RaiseTimerElapsed(j);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                byte[] picture = await _imageProcessor.DoTakePicture();
                pictures.Add(picture);
                RaiseImageChanged(picture, 1920, 1280); //todo take size from camera
                await Task.Delay(TimeSpan.FromSeconds(3)); //todo
                StartLiveView();
            }

            byte[] result = ProcessImages(pictures);
            return result;
        }

        private byte[] ProcessImages(List<byte[]> images)
        {
            int defWidth = 1024;
            int defHeight = 768;

            bool hasBackground = _pattern.Background != null;
            bool hasOverlay = _pattern.Overlay != null;

            MemoryStream backgroundStream = hasBackground ? new MemoryStream(_pattern.Background.Data) : null;
            MemoryStream overlayStream = hasOverlay ? new MemoryStream(_pattern.Overlay.Data) : null;
            Image backgroundImage = backgroundStream.Return(Image.FromStream, null);
            Image overlayImage = overlayStream.Return(Image.FromStream, null);

            int width = Math.Max(defWidth,
                Math.Max(backgroundImage.Return(x => x.Width, defWidth), overlayImage.Return(x => x.Width, defWidth)));

            int height = Math.Max(defHeight,
                Math.Max(backgroundImage.Return(x => x.Height, defHeight), overlayImage.Return(x => x.Height, defHeight)));

            using (Bitmap backgroundBitmap = new Bitmap(width, height))
            {
                using (Graphics canvas = Graphics.FromImage(backgroundBitmap))
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

// ReSharper disable once AccessToDisposedClosure
                    backgroundImage.Do(x => canvas.DrawImage(x,
                        new Rectangle(0, 0, width, height),
                        new Rectangle(0, 0, x.Width, x.Height), GraphicsUnit.Pixel));

                    List<TemplateImage> templates = _pattern.Images.ToList();

                    for (int i = 0; i < images.Count; i++)
                    {
                        TemplateImage template = templates[i];

                        int destWidth = (int) (width * template.Width);
                        int destHeight = (int) (height * template.Height);
                        int stepX = (int) (width * template.X);
                        int stepY = (int) (height * template.Y);

                        FillCanvas(canvas, images[i], new Point(stepX, stepY), destWidth, destHeight, 0, 0);
                    }

// ReSharper disable once AccessToDisposedClosure
                    overlayImage.Do(x => canvas.DrawImage(x,
                        new Rectangle(0, 0, width, height),
                        new Rectangle(0, 0, x.Width, x.Height), GraphicsUnit.Pixel));

                    canvas.Save();
                }

                return backgroundBitmap.ToBytes();
            }
        }

        protected void FillCanvas(
            Graphics canvas,
            byte[] buffer,
            Point position,
            int destWidth,
            int destHeight,
            int offsetX,
            int offsetY)
        {
            using (var imageStream = new MemoryStream(buffer))
            {
                Image image = Image.FromStream(imageStream);

                canvas.DrawImage(image,
                new Rectangle(position.X + offsetX, position.Y + offsetY, destWidth, destHeight),
                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
        }

        private void RaiseTimerElapsed(int tick)
        {
            var handler = TimerElapsed;
            if (handler != null)
                handler(this, tick);
        }

        private void RaiseImageChanged(byte[] imgBuf)
        {
            var handler = ImageChanged;
            if (handler != null)
                ImageChanged(this, new ImageDto(imgBuf));
        }

        private void RaiseImageChanged(byte[] imgBuf, int width, int height)
        {
            var handler = ImageChanged;
            if (handler != null)
                ImageChanged(this, new ImageDto(imgBuf, width, height));
        }

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
