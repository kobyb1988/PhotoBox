using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.ViewModels.Providers;

namespace ImageMaker.ViewModels.ViewModels
{
    public class ImportPatternsViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly PatternManageViewModelProvider _patternDataProvider;

        public ImportPatternsViewModel(
            IViewModelNavigator navigator, 
            IDialogService dialogService,
            PatternManageViewModelProvider patternDataProvider)
        {
            _navigator = navigator;
            _dialogService = dialogService;
            _patternDataProvider = patternDataProvider;
        }

        private IList<PatternManageViewModel> _patterns;

        public IEnumerable<PatternManageViewModel> Patterns
        {
            get { return _patterns ?? (_patterns = _patternDataProvider.Items.ToList()); }    
        } 

        private RelayCommand _goBackCommand;


        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        private void GoBack()
        {
            bool hasChanges = Patterns.Any(x => x.HasChanges);

            if (hasChanges)
            {
                bool result = _dialogService.ShowConfirmationDialog("При переходе все изменения будут потеряны. Продолжить?");
                if (!result)
                    return;    
            }

            _navigator.NavigateBack(this);
        }
    }
}
