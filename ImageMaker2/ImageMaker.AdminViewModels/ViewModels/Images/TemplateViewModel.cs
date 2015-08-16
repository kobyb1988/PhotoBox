using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class TemplateViewModel : BaseViewModel, ISelectable
    {
        protected uint _width;
        protected uint _height;
        private ObservableCollection<TemplateImageViewModel> _children;
        private bool _isSelected;
        private string _name;

        public TemplateViewModel(string name, uint width, uint height, int id, IEnumerable<TemplateImageViewModel> children)
        {
            Name = name;
            Id = id;
            _width = width;
            _height = height;
            foreach (var child in children)
            {
                Children.Add(child);
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
            Children.Add(new TemplateImageViewModel());   
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

    }
}
