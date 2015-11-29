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

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class CompositionModelProcessor
    {
        private readonly Template _pattern;
        private readonly ImageProcessor _imageProcessor;

        public event EventHandler<ImageDto> ImageChanged;
        public event EventHandler<int> TimerElapsed;
        public event EventHandler<int> ImageNumberChanged; 
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

        public virtual async Task<CompositionProcessingResult> TakePicture(byte[] liveViewImageStream)
        {
            Size liveViewImageStreamSize;
            using (var stream= new MemoryStream(liveViewImageStream))
            {
                var img = Image.FromStream(stream);
                liveViewImageStreamSize = img.Size;
            }
            
            return new CompositionProcessingResult(_pattern, await TakePictureInternal(liveViewImageStreamSize));
        }

        protected async Task<byte[]> TakePictureInternal(Size liveViewImageStreamSize)
        {
            return await Task.Run(() => Run(liveViewImageStreamSize));
        }

        private async Task<byte[]> Run(Size liveViewImageStreamSize)
        {
            List<byte[]> pictures = new List<byte[]>();

            for (int i = 0; i < _pattern.Images.Count; i++)
            {
                RaiseImageNumberChanged(i+1);
                RaiseTimerElapsed(4);
                await Task.Delay(TimeSpan.FromSeconds(2));

                for (int j = 3; j >= 0; j--)
                {
                    RaiseTimerElapsed(j);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                byte[] picture = await _imageProcessor.DoTakePicture();
                pictures.Add(picture);
                await Task.Delay(TimeSpan.FromSeconds(3)); //todo
                StopLiveView();
               StartLiveView();
            }

            byte[] result = ProcessImages(pictures,liveViewImageStreamSize);
            return result;
        }

        private byte[] ProcessImages(List<byte[]> images, Size liveViewImageStreamSize)
        {
            int defWidth = liveViewImageStreamSize.Width;//840;
            int defHeight = liveViewImageStreamSize.Height;//420; //768;

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

                        FillCanvas(canvas, images[i], new Point(stepX, stepY), destWidth, destHeight, 0, 0,liveViewImageStreamSize);
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

        protected void FillCanvas(Graphics canvas, byte[] buffer, Point position, int destWidth, int destHeight, int offsetX, int offsetY, Size liveViewImageStreamSize)
        {
            using (var imageStream = new MemoryStream(buffer))
            {
                Image image = Image.FromStream(imageStream);
                canvas.DrawImage(image,
                new Rectangle(position.X + offsetX, position.Y + offsetY, destWidth, destHeight),
                new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="hResolution"></param>
        /// <param name="vResolution"></param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height, float hResolution, float vResolution)
        {
            Rectangle cropRect = new Rectangle(0, 0, width, height);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height, image.PixelFormat);

            target.SetResolution(hResolution,vResolution);
            ((Bitmap) image).SetResolution(hResolution, vResolution);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(image, cropRect, new Rectangle(0, 0, width, height),
                                GraphicsUnit.Pixel);
            }
            return target;
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
