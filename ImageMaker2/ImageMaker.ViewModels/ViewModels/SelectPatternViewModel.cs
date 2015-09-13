using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.ViewModels.Providers;
using ImageMaker.ViewModels.ViewModels.Images;

namespace ImageMaker.ViewModels.ViewModels
{
    public class SelectPatternViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly PatternViewModelProvider _patternViewModelProvider;
        private ObservableCollection<TemplateViewModel> _patterns;
        private RelayCommand<TemplateViewModel> _selectPatternCommand;
        private RelayCommand _goBackCommand;
        private bool _isBusyLoading;
        private ICollectionView _patternsView;

        public SelectPatternViewModel(
            IViewModelNavigator navigator,
            PatternViewModelProvider patternViewModelProvider
            )
        {
            _navigator = navigator;
            _patternViewModelProvider = patternViewModelProvider;
        }

        public override void Initialize()
        {
            if (Patterns.Count > 0)
                return;

            IsBusyLoading = true;
            Task.Factory.StartNew(() => _patternViewModelProvider.GetPatternsAsync().Result)
                .ContinueWith(t =>
                {
                    t.Result.CopyTo(Patterns);
                    IsBusyLoading = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public ICollectionView PatternsView
        {
            get { return _patternsView ?? (_patternsView = CollectionViewSource.GetDefaultView(Patterns)); }
        }

        //public IList<CompositionViewModel> Patterns
        //{
        //    get { return _patterns ?? (_patterns = new ObservableCollection<CompositionViewModel>()); }
        //}

        public IList<TemplateViewModel> Patterns
        {
            get { return _patterns ?? (_patterns = new ObservableCollection<TemplateViewModel>()); }
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

        public RelayCommand<TemplateViewModel> SelectPatternCommand
        {
            get
            {
                return _selectPatternCommand ?? (_selectPatternCommand = new RelayCommand<TemplateViewModel>(SelectPattern));
            }
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        void SelectPattern(TemplateViewModel pattern)
        {
            //todo
            _navigator.NavigateForward<CameraViewModel>(this, pattern);
        }
    }
}
