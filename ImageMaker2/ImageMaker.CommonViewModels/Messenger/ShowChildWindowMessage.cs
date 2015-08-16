using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.CommonViewModels.Messenger
{
    public class ShowChildWindowMessage
    {
        public bool IsDialog { get; set; }

        public BaseViewModel Content { get; set; }
    }
}
