using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class CheckableImageWrapper : BaseViewModel
    {
        private bool _isChecked;
        public ImageViewModel Image { get; private set; }

        public CheckableImageWrapper(ImageViewModel image)
        {
            Image = image;
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value) return;

                _isChecked = value;
                RaisePropertyChanged();
            }
        }
    }
}
