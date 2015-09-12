using GalaSoft.MvvmLight.Command;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class StatsViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private RelayCommand _goBackCommand;

        public StatsViewModel(
            IViewModelNavigator navigator
            )
        {
            _navigator = navigator;
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }
    }
}
