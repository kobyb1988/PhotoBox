using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.PatternProcessing;
using ImageMaker.Utils.Services;

namespace ImageMaker.ViewModels.ViewModels
{
    public class CameraResultViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly ImagePrinter _printer;
        private readonly ImageService _imageService;
        private readonly string _printerName;

        public CameraResultViewModel(
            IViewModelNavigator navigator, 
            ImagePrinter printer, 
            SettingsProvider settingsProvider,
            ImageService imageService,
            CompositionProcessingResult result)
        {
            _navigator = navigator;
            _printer = printer;
            _imageService = imageService;
            Image = result.ImageResult;
            _copiesCount = 1;
            AppSettingsDto appSettings = settingsProvider.GetAppSettings();
            if (appSettings != null)
                _printerName = appSettings.PrinterName;
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        private RelayCommand _printImageCommand;
        private RelayCommand _goBackCommand;
        private byte[] _image;
        private int _copiesCount;

        public byte[] Image
        {
            get { return _image; }
            set { Set(() => Image, ref _image, value); }
        }

        public int CopiesCount
        {
            get { return _copiesCount; }
            set
            {
                _copiesCount = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand PrintImageCommand
        {
            get { return _printImageCommand ?? (_printImageCommand = new RelayCommand(Print)); }
        }

        private void Print()
        {
            _printer.Print(Image, _printerName, CopiesCount);
            _imageService.SaveImage(new ImageViewModel(Image));
        }
    }
}
