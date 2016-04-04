using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.ViewModels.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private RelayCommand _beginCommand;

        public WelcomeViewModel(
            IViewModelNavigator navigator)
        {
            _navigator = navigator;
        }

        public RelayCommand BeginCommand => _beginCommand ?? (_beginCommand = new RelayCommand(Begin));

        private void Begin()
        {
            _navigator.NavigateForward<SelectActivityViewModel>(this, null);
        }
    }
}
