using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Configuration;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageMaker.Camera;
using ImageMaker.CommonView.Helpers;
using ImageMaker.Entities;
using Image = System.Drawing.Image;
using Point = System.Drawing.Point;

namespace ImageMaker.PatternProcessing.ImageProcessors
{
    public class SimplePatternImageProcessor : PatternImageProcessor
    {
        public SimplePatternImageProcessor(PatternData pattern, ImageProcessor imageProcessor)
            : base(pattern, imageProcessor)
        {
        }

        protected override void TakePictureInternal(Action<byte[]> callback)
        {
            ImageProcessor.DoTakePicture(buffer =>
            {
                byte[] result = ProcessImage(buffer);
                callback(result);
            });
        }

        private byte[] ProcessImage(byte[] image)
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

                        int destWidth = backgroundBitmap.Width * 3 / 4;
                        int destHeight = backgroundBitmap.Height * 3 / 4;
                        int stepX = backgroundBitmap.Width / 6;
                        int stepY = backgroundBitmap.Height / 6;

                        FillCanvas(canvas, image, new Point(0, 0), destWidth, destHeight, 0, 0); //todo offset

                        canvas.Save();
                    }

                    return backgroundBitmap.ToBytes();
                }
            }
        }
    }
}