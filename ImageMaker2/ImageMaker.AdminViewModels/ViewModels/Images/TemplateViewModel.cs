using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ImageMaker.CommonViewModels.DragDrop;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class TemplateViewModel : BaseViewModel, ISelectable, IDropable
    {
        protected uint _width;
        protected uint _height;

        private ObservableCollection<TemplateImageViewModel> _children;
        private bool _isSelected;
        private string _name;

        private ImageViewModel _background;
        private ImageViewModel _overlay;

        public TemplateViewModel(string name, uint width, uint height, int id, IEnumerable<TemplateImageViewModel> children, ImageViewModel background, ImageViewModel overlay, bool isInstaPrinterTemplate)
        {
            Background = background;
            Overlay = overlay;

            Name = name;
            Id = id;
            IsInstaPrinterTemplate = isInstaPrinterTemplate;
            _width = width;
            _height = height;
            foreach (var child in children)
            {
                Children.Add(child);
                child.Index = Children.IndexOf(child) + 1;
            }
        }

        protected void RaiseSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null)
                handler(this);
        }

        public ObservableCollection<TemplateImageViewModel> Children
        {
            get { return _children ?? (_children = new ObservableCollection<TemplateImageViewModel>()); }
        }

        public void AddNewChild()
        {
            var child = new TemplateImageViewModel(Width, Height);
            Children.Add(child);
            child.Index = Children.IndexOf(child) + 1;
        }

        public int Id { get; protected set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public uint Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                    return;

                _width = value;
                RaisePropertyChanged();
            }
        }

        public uint Height
        {
            get { return _height; }
            set
            {
                if (_height == value)
                    return;

                _height = value;
                RaisePropertyChanged();
            }
        }

        private bool _isInstaPrinterTemplate;
        public bool IsInstaPrinterTemplate
        {
            get { return _isInstaPrinterTemplate; }
            set
            {
                _isInstaPrinterTemplate = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged();
            }
        }

        public event Action<ISelectable> SelectionChanged;

        public void SetSelected(bool status)
        {
            _isSelected = false;
            RaiseSelectionChanged();
            RaisePropertyChanged(() => IsSelected);
        }

        public Type DataType { get { return typeof (TemplateImageViewModel); } }

        public void Drop(object data)
        {
            return;
        }

        public ImageViewModel Background
        {
            get { return _background; }
            set
            {
                _background = value;
                RaisePropertyChanged();
            }
        }

        public ImageViewModel Overlay
        {
            get { return _overlay; }
            set
            {
                _overlay = value;
                RaisePropertyChanged();
            }
        }
    }
}
