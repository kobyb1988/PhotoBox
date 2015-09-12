namespace ImageMaker.CommonViewModels.ViewModels.Images
{
    public class ImageViewModel : BaseViewModel
    {
        public ImageViewModel(int id, string name, byte[] data) : this(data)
        {
            Name = name;
            Id = id;
        }

        public ImageViewModel(byte[] data)
        {
            Data = data;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public byte[] Data { get; private set; }
    }
}
