using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.PatternProcessing;
using ImageMaker.Utils.Services;
using System.IO;

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
            _maxCopies = 5;

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

        public RelayCommand CompleteCommand
        {
            get { return _completeCommand ?? (_completeCommand = new RelayCommand(Complete)); }
        }

        private void Complete()
        {
            _navigator.NavigateForward<SelectActivityViewModel>(this, null);
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        private RelayCommand _printImageCommand;
        private RelayCommand _goBackCommand;
        private byte[] _image;
        private int _width;
        private int _height;
        private int _copiesCount;
        private RelayCommand _completeCommand;
        private RelayCommand _changeUpCommand;
        private RelayCommand _changeDownCommand;
        private readonly int _maxCopies;

        public byte[] Image
        {
            get { return _image; }
            set {
                Set(() => Image, ref _image, value);

                var image = System.Drawing.Image.FromStream(new MemoryStream(Image));
                _width = image.Width;
                _height = image.Height;
            }
        }

        public int Width
        {
            get { return _width; }
        }
        public int Height
        {
            get { return _height; }
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

        public RelayCommand ChangeDownCommand
        {
            get
            {
                return _changeDownCommand ?? (_changeDownCommand = new RelayCommand(() =>
                                                                              {
                                                                                  CopiesCount--;
                                                                                  ChangeDownCommand.RaiseCanExecuteChanged();
                                                                              }, () => CopiesCount > 1)); }
        }

        public RelayCommand ChangeUpCommand
        {
            get { return _changeUpCommand ?? (_changeUpCommand = new RelayCommand(() =>
                                                                                  {
                                                                                      CopiesCount++;
                                                                                      ChangeUpCommand.RaiseCanExecuteChanged();
                                                                                  }, () => CopiesCount < _maxCopies)); }
        }


        public RelayCommand PrintImageCommand
        {
            get { return _printImageCommand ?? (_printImageCommand = new RelayCommand(Print)); }
        }

        private void Print()
        {
            _printer.Print(Image, _printerName, CopiesCount);
            _imageService.SaveImage(new ImageViewModel(Image));
            Complete();
        }
    }
}
