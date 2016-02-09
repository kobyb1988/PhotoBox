using System.Drawing;
using System.IO;

namespace ImageMaker.CommonViewModels.ViewModels.Images
{
    public class ImageViewModel  
    {
        public ImageViewModel(int id, string name, byte[] data) : this(data, name)
        {
            Id = id;
            CalcSize();
        }

        public ImageViewModel(byte[] data)
        {
            Data = data;
            CalcSize();
        }

        public ImageViewModel(byte[] data, string name) : this(data)
        {
            Name = name;
            CalcSize();
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public byte[] Data { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private void CalcSize()
        {
            if (Data == null)
                return;

            var image = Image.FromStream(new MemoryStream(Data));

            Width = image.Width;
            Height = image.Height;
        }
    }
}
