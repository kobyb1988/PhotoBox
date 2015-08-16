using System.Windows.Documents;

namespace ImageMaker.PatternProcessing.Dto
{
    public class ImageDto
    {
        public ImageDto(byte[] imageData)
        {
            ImageData = imageData;
            Width = 640;
            Height = 480;
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
