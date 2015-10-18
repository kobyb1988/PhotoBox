namespace ImageMaker.CommonViewModels.ViewModels.Images
{
    public class ImageViewModel  
    {
        public ImageViewModel(int id, string name, byte[] data) : this(data, name)
        {
            Id = id;
        }

        public ImageViewModel(byte[] data)
        {
            Data = data;
        }

        public ImageViewModel(byte[] data, string name) : this(data)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public byte[] Data { get; private set; }
    }
}
