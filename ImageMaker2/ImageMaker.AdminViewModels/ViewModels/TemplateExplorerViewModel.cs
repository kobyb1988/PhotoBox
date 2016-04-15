using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.AdminViewModels.Providers;
using ImageMaker.AdminViewModels.ViewModels.Enums;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class TemplateExplorerViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly TemplateViewModelProvider _viewModelProvider;
        private readonly IViewModelNavigator _navigator;
        private RelayCommand _addTemplateCommand;
        private RelayCommand _goBackCommand;
        private RelayCommand<FilterEventArgs> _filterCommand;
        private RelayCommand _updateTemplateCommand;
        private RelayCommand _setInstaPrinterTemplateCommand;
        private RelayCommand _saveCommand;
        private CheckableTemplateViewModel _selectedTemplate;
        private RelayCommand _checkCommand;

        private bool _dataLoaded;

        public TemplateExplorerViewModel(
            IDialogService dialogService,
            TemplateViewModelProvider viewModelProvider,
            IViewModelNavigator navigator)
        {
            _dialogService = dialogService;
            _viewModelProvider = viewModelProvider;
            _navigator = navigator;
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
                AddTemplateCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<CheckableTemplateViewModel> Children
            => _children ?? (_children = new ObservableCollection<CheckableTemplateViewModel>());

        public RelayCommand AddTemplateCommand
        {
            get { return _addTemplateCommand ?? (_addTemplateCommand = new RelayCommand(AddTemplate, () => !IsBusyLoading)); }
        }

        public RelayCommand RemoveTemplateCommand
        {
            get { return _removeTemplateCommand ?? (_removeTemplateCommand = new RelayCommand(RemoveTemplate, () => _canRemove)); }
        }

        public override void Initialize()
        {
            if (!_dataLoaded)
            {
                IsBusyLoading = true;

                Task.Factory.StartNew(() => _viewModelProvider.GetTemplatesAsync().Result)
                .ContinueWith(t =>
                {
                    t.Result.Select(x => x.ToCheckable()).CopyTo(Children);
                    IsBusyLoading = false;
                    _dataLoaded = true;
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (_updatedTemplate == null || _updatedTemplate.State == ItemState.Unchanged)
            {
                return;
            }

            if (!Children.Contains(_updatedTemplate))
                Children.Add(_updatedTemplate);

            CheckItem();
            _updatedTemplate = null;
        }

        private void RemoveTemplate()
        {
            //todo
            foreach (var child in Children.Where(x => x.IsChecked).ToList())
            {
                child.IsChecked = false;
                if (child.State == ItemState.Added)
                    Children.Remove(child);
                else
                {
                    child.State = ItemState.Removed;
                }
            }

            CheckItem();
        }

        public RelayCommand SetInstaPrinterTemplateCommand
        {
            get
            {
                return _setInstaPrinterTemplateCommand ?? (_setInstaPrinterTemplateCommand = new RelayCommand(SetInstaPrinterTemplate, () => SelectedTemplate != null));
            }
        }

        public RelayCommand UpdateTemplateCommand
        {
            get { return _updateTemplateCommand ?? (_updateTemplateCommand = new RelayCommand(UpdateTemplate, () => SelectedTemplate != null)); }
        }

        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, () => _canSave)); }
        }

        public RelayCommand CheckCommand => _checkCommand ?? (_checkCommand = new RelayCommand(CheckItem));

        private bool _canRemove;
        private bool _canSave;
        private RelayCommand _removeTemplateCommand;

        private void CheckItem()
        {
            _canRemove = Children.Any(x => x.IsChecked);
            _canSave = Children.Any(x => x.State != ItemState.Unchanged);

            UpdateCommands();
        }


        private void UpdateCommands()
        {
            RemoveTemplateCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            UpdateTemplateCommand.RaiseCanExecuteChanged();
        }

        private void Save()
        {
            var removed = Children.Where(x => x.State == ItemState.Removed).ToList();
            removed.ForEach(x => Children.Remove(x));

            var added = Children.Where(x => x.State == ItemState.Added || x.State == ItemState.UpdatedAdd).ToList();
            added.ForEach(x => x.State = ItemState.Unchanged);

            var updated = Children.Where(x => x.State == ItemState.Updated).ToList();
            updated.ForEach(x => x.State = ItemState.Unchanged);

            _viewModelProvider.RemoveTemplates(removed);
            _viewModelProvider.AddTemplates(added);
            _viewModelProvider.UpdateTemplates(updated);

            _viewModelProvider.SaveChanges();

            CheckItem();
        }
        private void SetInstaPrinterTemplate()
        {
            foreach (var child in Children)
            {
                child.IsInstaPrinterTemplate = child.IsChecked && !child.IsInstaPrinterTemplate;
                child.State=ItemState.Updated;
            }
            CheckItem();
        }

        private void UpdateTemplate()
        {
            _updatedTemplate = SelectedTemplate;
            _navigator.NavigateForward<TemplateEditorViewModel>(this, _updatedTemplate);
        }

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public CheckableTemplateViewModel SelectedTemplate
        {
            get { return _selectedTemplate; }
            set
            {
                if (_selectedTemplate == value)
                    return;

                _selectedTemplate = value;
                RaisePropertyChanged();
                UpdateCommands();
            }
        }

        public RelayCommand<FilterEventArgs> FilterCommand
            => _filterCommand ?? (_filterCommand = new RelayCommand<FilterEventArgs>(Filter));

        private void Filter(FilterEventArgs args)
        {
            var item = (CheckableTemplateViewModel) args.Item;
            args.Accepted = item.State != ItemState.Removed;
        }

        private void GoBack()
        {
            if (Children.Count(x => x.State == ItemState.Added) > 0)
            {
                bool result =
                    _dialogService.ShowConfirmationDialog("При переходе все изменения будут потеряны. Продолжить?");

                if (!result)
                    return;
                else
                    _navigator.NavigateBack(this);
            }
            Save();
            _navigator.NavigateBack(this);
        }

        private CheckableTemplateViewModel _updatedTemplate;
        private bool _isBusyLoading;
        private ObservableCollection<CheckableTemplateViewModel> _children;

        private void AddTemplate()
        {
            var viewModel = new ObjectNameViewModel("Введите имя шаблона");
            bool result = _dialogService.ShowResultDialog(viewModel);
            if (!result)
                return;

            string name = viewModel.Name;

            _updatedTemplate = new CheckableTemplateViewModel(name, 500, 500, 0,
                Enumerable.Empty<TemplateImageViewModel>(),
                TemplateEditorViewModel.CreateDefaultBackground(), null,false);

            _updatedTemplate.IsDefaultBackground = true;
            _navigator.NavigateForward<TemplateEditorViewModel>(this, _updatedTemplate);
        }
    }
}
