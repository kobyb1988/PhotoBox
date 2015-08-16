using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.ViewModels.ViewModels.Factories;

namespace ImageMaker.ViewModels.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IChildrenViewModelsFactory _viewModelFactory;
        private RelayCommand _beginCommand;

        public WelcomeViewModel(
            IViewModelNavigator navigator,
            IChildrenViewModelsFactory viewModelFactory)
        {
            _navigator = navigator;
            _viewModelFactory = viewModelFactory;
        }

        public RelayCommand BeginCommand
        {
            get { return _beginCommand ?? (_beginCommand = new RelayCommand(Begin)); }
        }

        private void Begin()
        {
            _navigator.NavigateForward(this, _viewModelFactory.GetChild<SelectActivityViewModel>(null));
        }
    }
}
