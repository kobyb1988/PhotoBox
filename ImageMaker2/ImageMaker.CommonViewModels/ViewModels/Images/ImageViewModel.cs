namespace ImageMaker.CommonViewModels.ViewModels.Images
{
    public class ImageViewModel : BaseViewModel
    {
        public ImageViewModel(int id, string name, byte[] data)
        {
            Name = name;
            Id = id;
            Data = data;
        }

        public string Name { get; private set; }
        public int Id { get; private set; }
        public byte[] Data { get; private set; }
    }
}
