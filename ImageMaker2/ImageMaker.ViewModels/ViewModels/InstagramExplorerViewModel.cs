using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Monads;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.ViewModels.ViewModels.Images;
using ImageMaker.WebBrowsing;

namespace ImageMaker.ViewModels.ViewModels
{
    public class InstagramExplorerViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly InstagramExplorer _instagramExplorer;

        public InstagramExplorerViewModel(
            IViewModelNavigator navigator, 
            InstagramExplorer instagramExplorer)
        {
            _navigator = navigator;
            _instagramExplorer = instagramExplorer;

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

                                  foreach (var image in task.Result.Return(x => x.Images, Enumerable.Empty<Image>()))
                                  {
                                      InstagramImageViewModel viewModel = new InstagramImageViewModel(image.Data,
                                          image.Width, image.Height, image.Url);
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
    }
}
