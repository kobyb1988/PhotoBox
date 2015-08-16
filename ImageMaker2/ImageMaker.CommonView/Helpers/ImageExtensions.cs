using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageMaker.CommonView.Helpers
{
    public static class ImageExtensions
    {
        public static byte[] ToBytes(this Bitmap bmp)
        {

            byte[] result = {};
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    result = ms.ToArray();
                }
            }
            catch (Exception e)
            {
            }

            return result;
        }

        public static ImageSource ToImage(this byte[] buffer)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(buffer);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            ImageSource imgSrc = biImg;

            return imgSrc;

            //BitmapImage result = new BitmapImage();
            //try
            //{
            //    using (MemoryStream ms = new MemoryStream(buffer))
            //    {
            //        result.BeginInit();
            //        result.StreamSource = ms;
            //        result.EndInit();
            //    }
            //}
            //catch (Exception)
            //{
            //}

            //return result;
        }
    }
}