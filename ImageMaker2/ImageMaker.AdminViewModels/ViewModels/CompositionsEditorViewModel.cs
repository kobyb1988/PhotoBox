using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using ImageMaker.AdminViewModels.ViewModels.Enums;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using Microsoft.Win32;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class CompositionsEditorViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;
        private RelayCommand _goBackCommand;

        private readonly CheckableCompositionViewModel _originalObject;
        private RelayCommand _addBackgroundCommand;
        private RelayCommand _addOverlayCommand;
        private RelayCommand _removeBackgroundCommand;
        private RelayCommand _removeOverlayCommand;

        public CompositionsEditorViewModel(
            IViewModelNavigator navigator,
            IDialogService dialogService, 
            CheckableCompositionViewModel compositionViewModel
            )
        {
            _navigator = navigator;
            _dialogService = dialogService;
            _originalObject = compositionViewModel;

            Composition = _originalObject.Copy();

            _canRemoveBackground = Composition.Background != null;
            _canRemoveOverlay = Composition.Overlay != null;
        }

        public CheckableCompositionViewModel Composition { get; private set; }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        public RelayCommand AddBackgroundCommand
        {
            get { return _addBackgroundCommand ?? (_addBackgroundCommand = new RelayCommand(AddBackground)); }
        }

        public RelayCommand AddOverlayCommand
        {
            get { return _addOverlayCommand ?? (_addOverlayCommand = new RelayCommand(AddOverlay)); }
        }

        private bool _canRemoveBackground;
        private bool _canRemoveOverlay;

        public RelayCommand RemoveBackgroundCommand
        {
            get { return _removeBackgroundCommand ?? (_removeBackgroundCommand = new RelayCommand(RemoveBackground, () => _canRemoveBackground)); }
        }

        public RelayCommand RemoveOverlayCommand
        {
            get { return _removeOverlayCommand ?? (_removeOverlayCommand = new RelayCommand(RemoveOverlay, () => _canRemoveOverlay)); }
        }

        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, () => _hasChanges)); }
            set { _saveCommand = value; }
        }

        private void Save()
        {
            if (Composition.State != ItemState.Added)
                Composition.State = ItemState.Updated;

            Composition.CopyTo(_originalObject);

            _hasChanges = false;
            UpdateCommands();
        }

        private void RemoveOverlay()
        {
            Composition.Overlay = null;
            _canRemoveOverlay = false;

            UpdateState();
            UpdateCommands();
        }

        private void RemoveBackground()
        {
            Composition.Background = null;
            _canRemoveBackground = false;

            UpdateState();
            UpdateCommands();
        }

        private void AddOverlay()
        {
            ImageViewModel overlay = LoadImage();
            if (overlay == null)
                return;
            
            Composition.Overlay = overlay;
            _canRemoveOverlay = true;

            UpdateState();
            UpdateCommands();
        }

        private void AddBackground()
        {
            ImageViewModel background = LoadImage();
            if (background == null)
                return;

            Composition.Background = background;
            _canRemoveBackground = true;

            UpdateState();
            UpdateCommands();
        }

        //todo undo stack
        private bool _hasChanges;
        private RelayCommand _saveCommand;

        private void UpdateState()
        {
            _hasChanges = false;

            if (Composition.State == ItemState.Added)
            {
                _hasChanges = true;
                return;
            }

            if (_originalObject.Background == null)
            {
                if (Composition.Background != null)
                {
                    _hasChanges = true;
                    return;
                }
            }
            else
            {
                if (Composition.Background == null)
                {
                    _hasChanges = true;
                    return;
                }

                if (Composition.Background.Id != _originalObject.Background.Id)
                {
                    _hasChanges = true;
                    return;
                }
            }

            if (_originalObject.Overlay == null)
            {
                if (Composition.Overlay != null)
                {
                    _hasChanges = true;
                    return;
                }
            }
            else
            {
                if (Composition.Overlay == null)
                {
                    _hasChanges = true;
                    return;
                }

                if (Composition.Overlay.Id != _originalObject.Overlay.Id)
                {
                    _hasChanges = true;
                    return;
                }
            }
        }

        private void UpdateCommands()
        {
            RemoveBackgroundCommand.RaiseCanExecuteChanged();
            RemoveOverlayCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void GoBack()
        {
            if (_hasChanges)
            {
                bool result = _dialogService.ShowConfirmationDialog("При переходе все изменения будут потеряны. Продолжить?");
                if (!result)
                    return;

                _originalObject.State = ItemState.Unchanged;
            }

            _navigator.NavigateBack(this);
        }

        private ImageViewModel LoadImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
                Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg"
            };

            Func<string, bool> isValidFile = file =>
            {
                string[] extensions =
                {
                    ".png",
                    ".jpeg",
                    ".jpg"
                };

                string ext = Path.GetExtension(file);
                return extensions.Contains(ext);
            };

            fileDialog.FileOk += (sender, args) =>
            {
                if (!fileDialog.FileNames.All(isValidFile))
                    args.Cancel = true;
            };

            bool? dialogResult = fileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                try
                {
                    Stream[] files = fileDialog.OpenFiles();
                    for (int i = 0; i < fileDialog.FileNames.Length; i++)
                    {
                        string filePath = fileDialog.FileNames[i];
                        Stream file = files[i];
                        byte[] fileData = null;

                        using (var stream = new MemoryStream())
                        {
                            file.CopyTo(stream);
                            fileData = stream.ToArray();
                        }

                        ImageViewModel viewModel = new ImageViewModel(0, Path.GetFileName(filePath), fileData);

                        return viewModel;
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return null;
        }
    }
}
