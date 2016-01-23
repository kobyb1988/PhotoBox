using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
    public class InstagramExplorerViewModel : BaseViewModel,ISearch
    {
        private readonly IViewModelNavigator _navigator;
        private readonly ImagePrinter _printer;
        private readonly PatternViewModelProvider _patternVmProvider;
        private readonly ImageUtils _imageUtils;
        private readonly IMappingEngine _mappingEngine;
        private readonly InstagramExplorer _instagramExplorer;
        private readonly string _printerName;

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

        public ICollectionView ImagesView
        {
            get { return _imagesView ?? (_imagesView = CollectionViewSource.GetDefaultView(Images)); }
        }

        public RelayCommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new RelayCommand(Print, () => _checkedImages.IsValueCreated && _checkedImages.Value.Count > 0)); }
        }

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

            foreach (var image in _checkedImages.Value)
            {
                byte[] imageData = new byte[] { };
                Size imageStreamSize;

                using (var stream = new MemoryStream(image.Data))
                {
                    var img = Image.FromStream(stream);
                    imageStreamSize = img.Size;
                }

                if (instaTemplate != null)
                    imageData = _imageUtils.ProcessImages(new List<byte[]> { image.Data }, imageStreamSize,
                        _mappingEngine.Map<EntityTemplate>(instaTemplate));

                else
                {
                    imageData = _imageUtils.GetCaptureForInstagramControl(image.Data,image.FullName,DateTime.Now,image.ProfilePictureData);
                }
                print(imageData);
            }
        }
      
        public ObservableCollection<InstagramImageViewModel> Images
        {
            get { return _images ?? (_images = new ObservableCollection<InstagramImageViewModel>()); }
        }

        private RelayCommand _goBackCommand;
        private bool _isHashTag;
        private bool _isUserName;
        private ObservableCollection<InstagramImageViewModel> _images;
        private RelayCommand<string> _searchCommand;
        private bool _isBusy;
        private ICollectionView _imagesView;

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack, () => !IsBusy)); }
        }

        public RelayCommand<string> SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand<string>(Search, (s) => !IsBusy)); }
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

        private string _nextUrl;
        private string _previousSearch;
        private RelayCommand _printCommand;

        public RelayCommand<InstagramImageViewModel> CheckCommand
        {
            get { return _checkCommand ?? (_checkCommand = new RelayCommand<InstagramImageViewModel>(Check)); }
        }

        private readonly Lazy<List<InstagramImageViewModel>> _checkedImages = new Lazy<List<InstagramImageViewModel>>();
        private RelayCommand<InstagramImageViewModel> _checkCommand;

        private void Check(InstagramImageViewModel image)
        {
            if (_checkedImages.Value.Contains(image))
            {
                _checkedImages.Value.Remove(image);
                return;
            }

            _checkedImages.Value.Add(image);
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

                                  foreach (var image in task.Result.Return(x => x.Images, Enumerable.Empty<WebBrowsing.Image>()))
                                  {
                                      InstagramImageViewModel viewModel = new InstagramImageViewModel(image.Data,
                                          image.Width, image.Height, image.Url,image.FullName, image.ProfilePictureData,image.UrlAvatar);
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

        private string _textSearch;

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
    }
}
