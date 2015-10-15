using GalaSoft.MvvmLight.Command;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class StatsViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private RelayCommand _goBackCommand;
        private RelayCommand _currentSessionCommand;
        private RelayCommand _allSessionsCommand;

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

        public RelayCommand CurrentSessionCommand
        {
            get { return _currentSessionCommand ?? (_currentSessionCommand = new RelayCommand(CurrentSession)); }
        }

        public RelayCommand AllSessionsCommand
        {
            get { return _allSessionsCommand ?? (_allSessionsCommand = new RelayCommand(AllSessions)); }
        }

        private void AllSessions()
        {
            throw new System.NotImplementedException();
        }

        private void CurrentSession()
        {
            _navigator.NavigateForward<CurrentSessionViewModel>(this, null);
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }
    }
}
