using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.AdminViewModels.ViewModels.Enums;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class TemplateEditorViewModel : BaseViewModel
    {
        internal static Lazy<StackStorage> Stack = new Lazy<StackStorage>();
 
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly ImageLoadService _imageLoadService;
        private RelayCommand _addImageCommand;
        private RelayCommand _removeImageCommand;
        private ISelectable _selectedObject;
        private RelayCommand _goBackCommand;
        private RelayCommand _undoCommand;
        private RelayCommand _redoCommand;
        private RelayCommand<ISelectable> _selectObjectCommand;

        private readonly CheckableTemplateViewModel _originalObject;
        private RelayCommand _saveCommand;
        private bool _canRemoveImage;
        private RelayCommand _addBackgroundCommand;
        private RelayCommand _addOverlayCommand;
        private RelayCommand _removeBackgroundCommand;
        private RelayCommand _removeOverlayCommand;
        private bool _canRemoveBackground;
        private bool _canRemoveOverlay;
        private double _overlayOpacity;


        public TemplateEditorViewModel(
            IViewModelNavigator navigator, 
            IDialogService dialogService, 
            ImageLoadService imageLoadService,
            CheckableTemplateViewModel template)
        {
            OverlayOpacity = 1.0;

            _navigator = navigator;
            _dialogService = dialogService;
            _imageLoadService = imageLoadService;
            _originalObject = template;
            Template = _originalObject.Copy();
            Init();
        }

        void Init()
        {
            foreach (var child in Template.Children)
            {
                child.SelectionChanged += ItemOnSelectionChanged;
            }

            Template.Children.CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in args.NewItems.OfType<ISelectable>())
                        {
                            item.SelectionChanged += ItemOnSelectionChanged;
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in args.OldItems.OfType<ISelectable>())
                        {
                            item.SelectionChanged -= ItemOnSelectionChanged;
                        }

                        if (Template.Children.Count == 0)
                        {
                            SelectedObject = null;
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        SelectedObject = null;
                        break;
                }
            };
        }

        private void ItemOnSelectionChanged(ISelectable item)
        {
            if (SelectedObject == item && !item.IsSelected)
            {
                UnselectCurrent();
            }

            //SelectedObject = item.IsSelected ? item : null;
        }

        private void UnselectCurrent()
        {
            _selectedObject = null;
            _canRemoveImage = false;
            RaisePropertyChanged(() => SelectedObject);
        }

        public double OverlayOpacity
        {
            get { return _overlayOpacity; }
            set
            {
                if (_overlayOpacity == value)
                    return;

                _overlayOpacity = value;
                RaisePropertyChanged();
            }
        }

        public ISelectable SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject == value)
                    return;

                TemplateImageViewModel oldOne = (TemplateImageViewModel)_selectedObject;
                TemplateImageViewModel newOne = (TemplateImageViewModel) value;

                if (oldOne != null)
                {
                    if (newOne != null)
                    {
                        Stack.Value.Chain(oldOne).Add(newOne);

                        oldOne.SetSelected(false);
                        newOne.SetSelected(true);
                    }
                }
                else
                {
                    if (newOne != null)
                        newOne.IsSelected = true;
                }

                _selectedObject = value;

                _canRemoveImage = _selectedObject is TemplateImageViewModel;
                UpdateCommands();

                RaisePropertyChanged();
            }
        }

        public CheckableTemplateViewModel Template { get; private set; }

        public RelayCommand AddImageCommand
        {
            get { return _addImageCommand ?? (_addImageCommand = new RelayCommand(AddImage)); }
        }

        public RelayCommand RemoveImageCommand
        {
            get { return _removeImageCommand ?? (_removeImageCommand = new RelayCommand(RemoveImage, () => _canRemoveImage)); }
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        public RelayCommand UndoCommand
        {
            get { return _undoCommand ?? (_undoCommand = new RelayCommand(Undo, () => Stack.IsValueCreated && Stack.Value.CanUndo)); }
        }

        public RelayCommand RedoCommand
        {
            get { return _redoCommand ?? (_redoCommand = new RelayCommand(Redo, () => Stack.IsValueCreated && Stack.Value.CanRedo)); }
        }

        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save, () => Stack.IsValueCreated && Stack.Value.CanUndo)); }
        }

        public RelayCommand AddBackgroundCommand
        {
            get { return _addBackgroundCommand ?? (_addBackgroundCommand = new RelayCommand(AddBackground)); }
        }

        public RelayCommand RemoveBackgroundCommand
        {
            get { return _removeBackgroundCommand ?? (_removeBackgroundCommand = new RelayCommand(RemoveBackground, () => _canRemoveBackground)); }
        }

        private void RemoveOverlay()
        {
            Stack.Value.Do(Template);

            Template.Overlay = null;
            _canRemoveOverlay = false;
            
            UpdateCommands();
        }

        private void RemoveBackground()
        {
            Stack.Value.Do(Template);

            Template.Background = CreateDefaultBackground();
            _canRemoveBackground = false;
            
            UpdateCommands();
        }

        public static ImageViewModel CreateDefaultBackground()
        {
            byte[] data = new byte[] { };
            using (Bitmap backgroundBitmap = new Bitmap(500, 500))
            {
                using (Graphics canvas = Graphics.FromImage(backgroundBitmap))
                {
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(255, 55, 62, 70);
                    System.Drawing.Brush pen = new SolidBrush(color);

                    canvas.FillRectangle(pen, 0, 0, 500, 500);
                }

                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        backgroundBitmap.Save(ms, ImageFormat.Png);
                        data = ms.ToArray();
                    }
                }
                catch (Exception e)
                {
                }
            }

            ImageViewModel background = new ImageViewModel(data);
            return background;
        }

        public RelayCommand RemoveOverlayCommand
        {
            get { return _removeOverlayCommand ?? (_removeOverlayCommand = new RelayCommand(RemoveOverlay, () => _canRemoveOverlay)); }
        }

        private void AddOverlay()
        {
            ImageViewModel overlay = _imageLoadService.TryLoadImage();
            if (overlay == null)
                return;

            Stack.Value.Do(Template);

            Template.Overlay = overlay;
            _canRemoveOverlay = true;

            UpdateCommands();
        }

        private void AddBackground()
        {
            ImageViewModel background = _imageLoadService.TryLoadImage();
            if (background == null)
                return;

            Stack.Value.Do(Template);

            Template.Background = background;
            _canRemoveBackground = true;
            
            UpdateCommands();
        }

        public RelayCommand AddOverlayCommand
        {
            get { return _addOverlayCommand ?? (_addOverlayCommand = new RelayCommand(AddOverlay)); }
        }


        private void Save()
        {
            if (Template.State != ItemState.Added)
                Template.State = ItemState.Updated;

            Template.CopyTo(_originalObject);

            Stack.Value.Clear();
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            RemoveImageCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();

            RemoveBackgroundCommand.RaiseCanExecuteChanged();
            RemoveOverlayCommand.RaiseCanExecuteChanged();
        }

        public RelayCommand<ISelectable> SelectObjectCommand
        {
            get { return _selectObjectCommand ?? (_selectObjectCommand = new RelayCommand<ISelectable>(SelectObject)); }
        }

        private void SelectObject(ISelectable obj)
        {
            if (obj == null)
            {
                if (SelectedObject != null)
                {
                    SelectedObject.IsSelected = false;
                }

                return;
            }

            if (SelectedObject != null && SelectedObject != obj)
            {
                TemplateImageViewModel oldOne = (TemplateImageViewModel) SelectedObject;
                TemplateImageViewModel newOne = (TemplateImageViewModel) obj;

                Stack.Value.Chain(oldOne).Add(newOne);
                oldOne.SetSelected(false);
                newOne.SetSelected(true);
                return;
            }

            obj.IsSelected = !obj.IsSelected;
        }

        private void Redo()
        {
            Stack.Value.Redo();
        }

        private void Undo()
        {
            Stack.Value.Undo();
        }

        private void GoBack()
        {
            if (Stack.IsValueCreated && Stack.Value.CanUndo)
            {
                bool result = _dialogService.ShowConfirmationDialog("При переходе все изменения будут потеряны. Продолжить?");
                if (!result)
                    return;

                Stack.Value.Reset();
                _originalObject.State = ItemState.Unchanged;
            }

            if (Stack.IsValueCreated)
                Stack.Value.Clear();

            _navigator.NavigateBack(this);
        }

        private void RemoveImage()
        {
            var image = (TemplateImageViewModel) SelectedObject;
            Stack.Value.Chain(image).Add(Template);
            image.SetSelected(false);
            
            UnselectCurrent();

            int index = Template.Children.IndexOf(image);
            for (int i = index + 1; i <= Template.Children.Count - 1; i++)
                Template.Children.ElementAt(i).Index--;

            Template.Children.Remove(image);
        }

        private void AddImage()
        {
            Stack.Value.Do(Template);
            Template.AddNewChild();
        }
    }
}
