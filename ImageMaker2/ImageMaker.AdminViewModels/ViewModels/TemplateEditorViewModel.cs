using System;
using System.Collections.Specialized;
using System.Diagnostics;
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

        private void Init()
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

            _canRemoveBackground = !Template.IsDefaultBackground;
            _canRemoveOverlay = Template.Overlay != null;
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
                if (Math.Abs(_overlayOpacity - value) < 0.001)
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

                var oldOne = (TemplateImageViewModel) _selectedObject;
                var newOne = (TemplateImageViewModel) value;

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

        public RelayCommand AddImageCommand => _addImageCommand ?? (_addImageCommand = new RelayCommand(AddImage));

        public RelayCommand RemoveImageCommand
        {
            get { return _removeImageCommand ?? (_removeImageCommand = new RelayCommand(RemoveImage, () => _canRemoveImage)); }
        }

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

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
            => _addBackgroundCommand ?? (_addBackgroundCommand = new RelayCommand(AddBackground));

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
            Template.IsDefaultBackground = true;

            _canRemoveBackground = false;
            
            UpdateCommands();
        }

        public static ImageViewModel CreateDefaultBackground()
        {
            var data = new byte[] {};
            using (var backgroundBitmap = new Bitmap(500, 500))
            {
                using (var canvas = Graphics.FromImage(backgroundBitmap))
                {
                    var color = System.Drawing.Color.FromArgb(255, 55, 62, 70);
                    var pen = new SolidBrush(color);

                    canvas.FillRectangle(pen, 0, 0, 500, 500);
                }

                try
                {
                    using (var ms = new MemoryStream())
                    {
                        backgroundBitmap.Save(ms, ImageFormat.Png);
                        data = ms.ToArray();
                    }
                }
                catch (Exception e)
                {
                }
            }

            var background = new ImageViewModel(data);
            return background;
        }

        public RelayCommand RemoveOverlayCommand
        {
            get { return _removeOverlayCommand ?? (_removeOverlayCommand = new RelayCommand(RemoveOverlay, () => _canRemoveOverlay)); }
        }

        private void AddOverlay()
        {
            var overlay = _imageLoadService.TryLoadImage();
            if (overlay == null)
                return;

            Stack.Value.Do(Template);

            Template.Overlay = overlay;
            _canRemoveOverlay = true;

            UpdateCommands();
        }

        private void AddBackground()
        {
            var background = _imageLoadService.TryLoadImage();
            if (background == null)
                return;

            Template.IsDefaultBackground = false;
            Stack.Value.Do(Template);

            Template.Background = background;
            _canRemoveBackground = true;
            
            UpdateCommands();
        }

        public RelayCommand AddOverlayCommand
            => _addOverlayCommand ?? (_addOverlayCommand = new RelayCommand(AddOverlay));


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
            => _selectObjectCommand ?? (_selectObjectCommand = new RelayCommand<ISelectable>(SelectObject));

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
                var oldOne = (TemplateImageViewModel) SelectedObject;
                var newOne = (TemplateImageViewModel) obj;

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

            var index = Template.Children.IndexOf(image);
            for (var i = index + 1; i <= Template.Children.Count - 1; i++)
                Template.Children.ElementAt(i).Index--;

            Template.Children.Remove(image);
        }

        private void AddImage()
        {
            Stack.Value.Do(Template);
            Template.AddNewChild();
        }

        private bool IsImageSelected()
        {
            return SelectedObject != null;
        }

        #region Move Commands

        private RelayCommand _moveByOnePixelTopCommand;
        private RelayCommand _moveByOnePixelRightCommand;
        private RelayCommand _moveByOnePixelBottomCommand;
        private RelayCommand _moveByOnePixelLeftCommand;

        public RelayCommand MoveByOnePixelTopCommand
            =>
                _moveByOnePixelTopCommand ??
                (_moveByOnePixelTopCommand = new RelayCommand(MoveByOnePixelTop, IsImageSelected));
        public RelayCommand MoveByOnePixelRightCommand
            =>
                _moveByOnePixelRightCommand ??
                (_moveByOnePixelRightCommand = new RelayCommand(MoveByOnePixelRight, IsImageSelected));
        public RelayCommand MoveByOnePixelBottomCommand
            =>
                _moveByOnePixelBottomCommand ??
                (_moveByOnePixelBottomCommand = new RelayCommand(MoveByOnePixelBottom, IsImageSelected));
        public RelayCommand MoveByOnePixelLeftCommand
            =>
                _moveByOnePixelLeftCommand ??
                (_moveByOnePixelLeftCommand = new RelayCommand(MoveByOnePixelLeft, IsImageSelected));

        private void MoveByOnePixelTop()
        {
            var image = (TemplateImageViewModel) SelectedObject;
            image.Move(0, -1);
        }
        private void MoveByOnePixelRight()
        {
            var image = (TemplateImageViewModel) SelectedObject;
            image.Move(1, 0);
        }
        private void MoveByOnePixelBottom()
        {
            var image = (TemplateImageViewModel) SelectedObject;
            image.Move(0, 1);
        }
        private void MoveByOnePixelLeft()
        {
            var image = (TemplateImageViewModel) SelectedObject;
            image.Move(-1, 0);
        }

        #endregion

        #region Increase Commands

        private RelayCommand _increaseByOnePixelLtCommand;
        private RelayCommand _increaseByOnePixelRtCommand;
        private RelayCommand _increaseByOnePixelRbCommand;
        private RelayCommand _increaseByOnePixelLbCommand;

        public RelayCommand IncreaseByOnePixelLtCommand
            =>
                _increaseByOnePixelLtCommand ??
                (_increaseByOnePixelLtCommand = new RelayCommand(IncreaseByOnePixelLt, IsImageSelected));
        public RelayCommand IncreaseByOnePixelRtCommand
            =>
                _increaseByOnePixelRtCommand ??
                (_increaseByOnePixelRtCommand = new RelayCommand(IncreaseByOnePixelRt, IsImageSelected));
        public RelayCommand IncreaseByOnePixelRbCommand
            =>
                _increaseByOnePixelRbCommand ??
                (_increaseByOnePixelRbCommand = new RelayCommand(IncreaseByOnePixelRb, IsImageSelected));
        public RelayCommand IncreaseByOnePixelLbCommand
            =>
                _increaseByOnePixelLbCommand ??
                (_increaseByOnePixelLbCommand = new RelayCommand(IncreaseByOnePixelLb, IsImageSelected));

        private void IncreaseByOnePixelLt()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(1, 1, -1, -1);
        }
        private void IncreaseByOnePixelRt()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(1, 1, 0, -1);
        }
        private void IncreaseByOnePixelRb()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(1, 1, 0, 0);
        }
        private void IncreaseByOnePixelLb()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(1, 1, -1, 0);
        }

        #endregion

        #region Decrease Commands

        private RelayCommand _decreaseByOnePixelLtCommand;
        private RelayCommand _decreaseByOnePixelRtCommand;
        private RelayCommand _decreaseByOnePixelRbCommand;
        private RelayCommand _decreaseByOnePixelLbCommand;

        public RelayCommand DecreaseByOnePixelLtCommand
            =>
                _decreaseByOnePixelLtCommand ??
                (_decreaseByOnePixelLtCommand = new RelayCommand(DecreaseByOnePixelLt, IsImageSelected));
        public RelayCommand DecreaseByOnePixelRtCommand
            =>
                _decreaseByOnePixelRtCommand ??
                (_decreaseByOnePixelRtCommand = new RelayCommand(DecreaseByOnePixelRt, IsImageSelected));
        public RelayCommand DecreaseByOnePixelRbCommand
            =>
                _decreaseByOnePixelRbCommand ??
                (_decreaseByOnePixelRbCommand = new RelayCommand(DecreaseByOnePixelRb, IsImageSelected));
        public RelayCommand DecreaseByOnePixelLbCommand
            =>
                _decreaseByOnePixelLbCommand ??
                (_decreaseByOnePixelLbCommand = new RelayCommand(DecreaseByOnePixelLb, IsImageSelected));

        private void DecreaseByOnePixelLt()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(-1, -1, 1, 1);
        }
        private void DecreaseByOnePixelRt()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(-1, -1, 0, 1);
        }
        private void DecreaseByOnePixelRb()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(-1, -1, 0, 0);
        }
        private void DecreaseByOnePixelLb()
        {
            var image = (TemplateImageViewModel)SelectedObject;
            image.Resize(-1, -1, 1, 0);
        }

        #endregion
    }
}
