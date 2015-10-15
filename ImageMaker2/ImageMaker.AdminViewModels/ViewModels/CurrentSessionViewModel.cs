using System.Collections.ObjectModel;
using System.Monads;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using ImageMaker.AdminViewModels.Services;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class CurrentSessionViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SessionService _sessionService;
        private RelayCommand _goBackCommand;
        private ObservableCollection<ImageViewModel> _images;
        private bool _isBusyLoading;

        public CurrentSessionViewModel(
            IViewModelNavigator navigator,
            SessionService sessionService
            )
        {
            _navigator = navigator;
            _sessionService = sessionService;
        }

        public override void Initialize()
        {
            IsBusyLoading = true;
            _sessionService.GetImagesAsync()
                .ContinueWith(t =>
                              {
                                  t.Result.Do(x => x.CopyTo(Images));
                                  IsBusyLoading = false;
                              }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        public ObservableCollection<ImageViewModel> Images
        {
            get { return _images ?? (_images = new ObservableCollection<ImageViewModel>()); }
        }

        public bool IsBusyLoading
        {
            get { return _isBusyLoading; }
            set
            {
                if (_isBusyLoading == value)
                    return;

                _isBusyLoading = value;
                RaisePropertyChanged();
            }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }
    }
}
