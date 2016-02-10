using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Monads;
using System.Threading.Tasks;
using System.Windows.Data;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.Utils.Services;
using ImageMaker.ViewModels.Abstract;
using ImageMaker.ViewModels.Providers;
using ImageMaker.ViewModels.ViewModels.Images;
using ImageMaker.WebBrowsing;
using EntityTemplate = ImageMaker.Entities.Template;
using Image = System.Drawing.Image;

namespace ImageMaker.ViewModels.ViewModels
{
    public class InstagramExplorerViewModel : BaseViewModel, ISearch
    {
        #region Properties And Fields
        private readonly IViewModelNavigator _navigator;
        private readonly ImagePrinter _printer;
        private readonly PatternViewModelProvider _patternVmProvider;
        private readonly ImageUtils _imageUtils;
        private readonly IMappingEngine _mappingEngine;
        private readonly InstagramExplorer _instagramExplorer;
        private readonly string _printerName;

        private RelayCommand _goBackCommand;
        private bool _isHashTag;
        private bool _isUserName;
        private ObservableCollection<InstagramImageViewModel> _images;
        private RelayCommand<string> _searchCommand;
        private bool _isBusy;
        private ICollectionView _imagesView;

        private string _nextUrl;
        private string _previousSearch;

        private InstagramImageViewModel _checkedImage;
        private RelayCommand<InstagramImageViewModel> _checkCommand;

        private string _textSearch;
        private string _lastInstagramImageId;


        public bool IsHashTag
        {
            get { return _isHashTag; }
            set
            {
                if (_isHashTag == value)
                    return;

                _isHashTag = value;
                _nextUrl = null;
                Images.Clear();
                RaisePropertyChanged();
            }
        }

        public bool IsUserName
        {
            get { return _isUserName; }
            set
            {
                if (_isUserName == value)
                    return;

                _isUserName = value;
                _nextUrl = null;
                Images.Clear();
                RaisePropertyChanged();
            }
        }


        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                    return;

                Set(() => IsBusy, ref _isBusy, value);
                UpdateCommands();
            }
        }

        public string TextSearch
        {
            get
            {
                return _textSearch;
            }
            set
            {
                _textSearch = value;
                RaisePropertyChanged();
            }
        }

        public ICollectionView ImagesView => _imagesView ?? (_imagesView = CollectionViewSource.GetDefaultView(Images));
        public ObservableCollection<InstagramImageViewModel> Images => _images ?? (_images = new ObservableCollection<InstagramImageViewModel>());

        #endregion
        
        public InstagramExplorerViewModel(
            IViewModelNavigator navigator,
            InstagramExplorer instagramExplorer,
            SettingsProvider settings,
            ImagePrinter printer, PatternViewModelProvider patternVMProvider,
            ImageUtils imageUtils, IMappingEngine mappingEngine)
        {
            _navigator = navigator;
            _printer = printer;
            _patternVmProvider = patternVMProvider;
            _imageUtils = imageUtils;
            _mappingEngine = mappingEngine;
            _instagramExplorer = instagramExplorer;
            AppSettingsDto appSettings = settings.GetAppSettings();
            if (appSettings != null)
                _printerName = appSettings.PrinterName;

            IsHashTag = true;

        }

        #region Methods
        private async void Print()
        {
            var result = await _patternVmProvider.GetPatternsAsync();

            TemplateViewModel instaTemplate = result.SingleOrDefault(x => x.IsInstaPrinterTemplate);
            Action<byte[]> print = null;
            if (!string.IsNullOrEmpty(_printerName))
                print = (data) => _printer.Print(data, _printerName);
            else
            {
                print = (data) => _printer.Print(data);
            }

            byte[] imageData = new byte[] { };
            Size imageStreamSize;

            using (var stream = new MemoryStream(_checkedImage.Data))
            {
                var img = Image.FromStream(stream);
                imageStreamSize = img.Size;
            }

            if (instaTemplate != null)
                imageData = _imageUtils.ProcessImages(new List<byte[]> { _checkedImage.Data }, imageStreamSize,
                    _mappingEngine.Map<EntityTemplate>(instaTemplate));

            else
            {
                imageData = _imageUtils.GetCaptureForInstagramControl(_checkedImage.Data, _checkedImage.FullName, DateTime.Now, _checkedImage.ProfilePictureData);
            }
            print(imageData);

            _navigator.NavigateForward<SelectActivityViewModel>(this, null);
        }
        private void Check(InstagramImageViewModel image)
        {
            _checkedImage = image;
            foreach (var imageViewModel in Images.Where(x=>x!=_checkedImage))
            {
                imageViewModel.IsChecked = false;
            }
            PrintCommand.RaiseCanExecuteChanged();
        }

        private void Search(string text)
        {
            if (!string.IsNullOrEmpty(_previousSearch))
            {
                if (string.Compare(text, _previousSearch, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    _nextUrl = null;
                    Images.Clear();
                }
            }

            _previousSearch = text;

            IsBusy = true;
            SearchCommand.RaiseCanExecuteChanged();
            Task<ImageResponse> task = IsHashTag
                ? (string.IsNullOrEmpty(_nextUrl)
                    ? _instagramExplorer.GetImagesByHashTag(text, null)
                    : _instagramExplorer.GetImagesFromUrl(_nextUrl))
                : (string.IsNullOrEmpty(_nextUrl)
                    ? _instagramExplorer.GetImagesByUserName(text, null)
                    : _instagramExplorer.GetImagesFromUrl(_nextUrl));

            task.ContinueWith(t =>
            {
                // _images.Clear();
                _nextUrl = task.Result.Return(x => x.NextUrl, null);

                if (_nextUrl == null && _lastInstagramImageId == task.Result.MinTagId)
                {
                    IsBusy = false;
                    SearchCommand.RaiseCanExecuteChanged();
                    return;
                }

                _lastInstagramImageId = task.Result.MinTagId;
                foreach (var image in task.Result.Return(x => x.Images, Enumerable.Empty<WebBrowsing.Image>()))
                {
                    InstagramImageViewModel viewModel = new InstagramImageViewModel(image.Data,
                        image.Width, image.Height, image.Url, image.FullName, image.ProfilePictureData, image.UrlAvatar);
                    _images.Add(viewModel);
                }

                IsBusy = false;
                SearchCommand.RaiseCanExecuteChanged();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateCommands()
        {
            GoBackCommand.RaiseCanExecuteChanged();
            SearchCommand.RaiseCanExecuteChanged();
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }
        #endregion

        #region Commands
        public RelayCommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new RelayCommand(Print, () => _checkedImage != null && Images.Any(x=>x.IsChecked))); }
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack, () => !IsBusy)); }
        }

        public RelayCommand<string> SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand<string>(Search, (s) => !IsBusy)); }
        }
        private RelayCommand _printCommand;

        public RelayCommand<InstagramImageViewModel> CheckCommand => _checkCommand ?? (_checkCommand = new RelayCommand<InstagramImageViewModel>(Check));

        #endregion
    }
}
