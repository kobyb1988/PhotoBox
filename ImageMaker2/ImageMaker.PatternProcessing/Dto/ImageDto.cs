using System.Drawing;
using System.IO;
using System.Windows.Documents;

namespace ImageMaker.PatternProcessing.Dto
{
    public class ImageDto
    {
        private static object _lockImageData= new object();
        public ImageDto(byte[] imageData)
        {
            ImageData = imageData;
            Width = 0;//640;//это просто дефолтные настройки по умолчанию
            Height = 0;// 480;//это просто дефолтные настройки по умолчанию
            if (imageData.Length > 0)
                //lock (_lockImageData)
                //{
                using (var stream = new MemoryStream(imageData))
                {
                    var image = Image.FromStream(stream);
                    Width = image.Width;
                    Height = image.Height;
                }
            //}
        }

        public ImageDto(byte[] imageData, int width, int height) : this(imageData)
        {
            Width = width;
            Height = height;
        }

        public byte[] ImageData { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }
    }
}
