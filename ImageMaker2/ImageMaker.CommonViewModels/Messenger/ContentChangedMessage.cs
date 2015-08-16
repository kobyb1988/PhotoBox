using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.CommonViewModels.Messenger
{
    public class ContentChangedMessage
    {
        public ContentChangedMessage()
        {
        }

        public object Parameter { get; set; }

        public BaseViewModel Content { get; set; }
    }
}
