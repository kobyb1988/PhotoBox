using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.PatternProcessing;
using ImageMaker.Utils.Services;
using System.IO;
using NLog;

namespace ImageMaker.ViewModels.ViewModels
{
    public class CameraResultViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly ImagePrinter _printer;
        private readonly ImageService _imageService;
        private readonly string _printerName;
        private Logger _logger = LogManager.GetCurrentClassLogger();

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
            var appSettings = settingsProvider.GetAppSettings();
            if (appSettings != null)
            {
                _maxCopies = appSettings.MaxPrinterCopies;
                _printerName = appSettings.PrinterName;
            }
        }

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public RelayCommand CompleteCommand => _completeCommand ?? (_completeCommand = new RelayCommand(Complete));

        private void Complete()
        {
            _navigator.NavigateForward<SelectActivityViewModel>(this, null);
        }

        private void GoBack()
        {
            _navigator.NavigateForward<SelectPatternViewModel>(this, null);
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
                if (value != null)
                {
                    Set(() => Image, ref _image, value);

                    var image = System.Drawing.Image.FromStream(new MemoryStream(Image));
                    _width = image.Width;
                    _height = image.Height;
                }
                else _logger.Trace("Попытка сохранить в объект Image null.");
            }
        }

        public int Width => _width;

        public int Height => _height;

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
                }, () => CopiesCount > 1));
            }
        }

        public RelayCommand ChangeUpCommand
        {
            get
            {
                return _changeUpCommand ?? (_changeUpCommand = new RelayCommand(() =>
                {
                    CopiesCount++;
                    ChangeUpCommand.RaiseCanExecuteChanged();
                }, () => CopiesCount < _maxCopies));
            }
        }


        public RelayCommand PrintImageCommand => _printImageCommand ?? (_printImageCommand = new RelayCommand(Print));

        private void Print()
        {
            _printer.Print(Image, _printerName, CopiesCount);
            _imageService.SaveImage(new ImageViewModel(Image));
            Complete();
        }
    }
}
