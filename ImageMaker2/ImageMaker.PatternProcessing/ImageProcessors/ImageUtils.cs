﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Monads;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageMaker.CommonView.Helpers;
using ImageMaker.Entities;
using ImageMaker.PatternProcessing.ProcessingViews;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class ImageUtils
    {
        public byte[] GetCaptureForInstagramControl(byte[] imageForInstagram, string publishAuthorName, DateTime timeUpdate, byte[] profilePictureData)
        {
            byte[] captureImageData;
            var defaultTemplate = new InstagramDefaultCtrl();
            defaultTemplate.FillData(imageForInstagram,publishAuthorName,timeUpdate,profilePictureData);
            defaultTemplate.Measure(new System.Windows.Size((int)defaultTemplate.Width, (int)defaultTemplate.Height));
            defaultTemplate.Arrange(new Rect(new System.Windows.Size((int)defaultTemplate.Width, (int)defaultTemplate.Height)));

            int dpiX = 96;
            int dpiY = 96;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(defaultTemplate);
            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                           (int)(bounds.Height * dpiY / 96.0), dpiX, dpiY, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(defaultTemplate);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            renderTarget.Render(dv);

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = 100;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                captureImageData = outputStream.ToArray();
            }
            using (Bitmap rectangleResultImage = new Bitmap((int)bounds.Width, (int)bounds.Height))
            {
                using (Graphics canvas = Graphics.FromImage(rectangleResultImage))
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    using (var imageStream = new MemoryStream(captureImageData))
                    {
                        System.Drawing.Image squereResultImage = System.Drawing.Image.FromStream(imageStream);
                        canvas.DrawImage(squereResultImage,
                        new Rectangle(0, 0, squereResultImage.Width, squereResultImage.Height),
                        new Rectangle(0, 0, squereResultImage.Width, squereResultImage.Height), GraphicsUnit.Pixel);
                    }

                    return rectangleResultImage.ToBytes();
                }
            }
        }
        public byte[] ProcessImages(List<byte[]> images, Size liveViewImageStreamSize, Template pattern)
        {
            int defWidth = liveViewImageStreamSize.Width;//840;
            int defHeight = liveViewImageStreamSize.Height;//420; //768;

            bool hasBackground = pattern.Background != null;
            bool hasOverlay = pattern.Overlay != null;

            MemoryStream backgroundStream = hasBackground ? new MemoryStream(pattern.Background.Data) : null;
            MemoryStream overlayStream = hasOverlay ? new MemoryStream(pattern.Overlay.Data) : null;
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

                    List<TemplateImage> templates = pattern.Images.ToList();

                    for (int i = 0; i < images.Count; i++)
                    {
                        TemplateImage template = templates[i];

                        int destWidth = (int)(width * template.Width);
                        int destHeight = (int)(height * template.Height);
                        int stepX = (int)(width * template.X);
                        int stepY = (int)(height * template.Y);

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

        protected void FillCanvas(Graphics canvas, byte[] buffer, Point position, int destWidth, int destHeight, int offsetX, int offsetY)
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

            target.SetResolution(hResolution, vResolution);
            ((Bitmap)image).SetResolution(hResolution, vResolution);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(image, cropRect, new Rectangle(0, 0, width, height),
                                GraphicsUnit.Pixel);
            }
            return target;
        }

        public Image ScaleByHeightAndResolution(Image img, int height, float hResolution, float vResolution)
        {
            double fractionalPercentage = ((double)height / (double)img.Height);
            int outputWidth = (int)(img.Width * fractionalPercentage);
            int outputHeight = height;
            return ResizeImage(img, outputWidth, outputHeight, hResolution, vResolution);
        }

        //public static Image ScaleImageAndResolution(Image img, int outputWidth, int outputHeight, float Resolution)
        //{
        //    Bitmap outputImage = new Bitmap(outputWidth, outputHeight, img.PixelFormat);
        //    outputImage.SetResolution(Resolution, Resolution);
        //    Graphics graphics = Graphics.FromImage(outputImage);
        //    Graphics.InterpolationMode = InterpolationMode.Bilinear;
        //    graphics.DrawImage(img, new Rectangle(0, 0, outputWidth, outputHeight), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        //    graphics.Dispose();
        //    return outputImage;
        //}
    }
}