using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using ImageMaker.Camera;
using ImageMaker.CommonView.Helpers;
using ImageMaker.Entities;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class StripeImageProcessor : PatternImageProcessor
    {
        public StripeImageProcessor(PatternData pattern, ImageProcessor imageProcessor)
            : base(pattern, imageProcessor)
        {
        }

        protected override void TakePictureInternal(Action<byte[]> callback)
        {
            List<byte[]> pictures = new List<byte[]>();

            var timer = new System.Timers.Timer(3000);

            int count = 0;
            timer.Elapsed += (sender, args) =>
            {
                ++count;
                if (count > 3)
                {
                    timer.Stop();
                    byte[] result = ProcessImages(pictures);
                    callback(result);
                    return;
                }

                timer.Stop();
                ImageProcessor.DoTakePicture(stream =>
                {
                    pictures.Add(stream);
                    timer.Start();
                });
            };

            timer.Start();
        }

        private byte[] ProcessImages(List<byte[]> images)
        {
            using (MemoryStream backgroundStream = new MemoryStream(Pattern.Data))
            {
                Image backgroundImage = Image.FromStream(backgroundStream);
                using (Bitmap backgroundBitmap = new Bitmap(backgroundImage.Width, backgroundImage.Height))
                {
                    using (Graphics canvas = Graphics.FromImage(backgroundBitmap))
                    {
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.DrawImage(backgroundImage, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height),
                            new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), GraphicsUnit.Pixel);

                        int destWidth = backgroundBitmap.Width / 5;
                        int destHeight = backgroundBitmap.Height / 5;
                        int stepX = backgroundBitmap.Width / 6;
                        int stepY = backgroundBitmap.Height / 6;

                        FillCanvas(canvas, images[0], new Point(stepX, stepY), destWidth, destHeight, 0, 0);
                        FillCanvas(canvas, images[1], new Point(2 * stepX, stepY), destWidth, destHeight, destWidth, 0);
                        FillCanvas(canvas, images[2], new Point(stepX, 2 * stepY), destWidth, destHeight, 0, destHeight);

                        canvas.Save();
                    }

                    return backgroundBitmap.ToBytes();
                }
            }
        }
    }
}