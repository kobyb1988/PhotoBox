using System;
using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.DragDrop;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class TemplateImageViewModel : BaseViewModel, ICopyable<TemplateImageViewModel>, ISelectable, IResizable, IDragable
    {
        private double _parentWidth;
        private double _parentHeight;

        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private bool _isSelected;

        public TemplateImageViewModel(double x, double y, double width, double height, int id, double parentWidth, double parentHeight) 
            : this(parentWidth, parentHeight)
        {
            Id = id;
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public TemplateImageViewModel(double parentWidth, double parentHeight)
        {
            _parentHeight = parentHeight;
            _parentWidth = parentWidth;
            _x = 0;
            _y = 0;
            _width = 0.1;
            _height = 0.1;
        }

        public double X
        {
            get { return _x; }
            set
            {
                if (_x == value)
                    return;

                
                PushState();

                _x = value;
                RaisePropertyChanged(() => X);
                RaiseSelectionChanged();
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (_y == value)
                    return;

                
                PushState();

                _y = value;
                RaisePropertyChanged(() => Y);
                RaiseSelectionChanged();
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                    return;
                
                PushState();

                _width = value;
                RaisePropertyChanged(() => Width);
                RaiseSelectionChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (_height == value)
                    return;

                
                PushState();
                _height = value;
                RaisePropertyChanged(() => Height);
                RaiseSelectionChanged();
            }
        }

        public int Id { get; private set; }

        private void PushState()
        {
            TemplateEditorViewModel.Stack.Value.Do(this);
        }

        protected void RaiseSelectionChanged()
        {
            var handler = SelectionChanged;
            if (handler != null)
                handler(this);
        }

        private void UpdateProperties()
        {
            RaisePropertyChanged(() => X);
            RaisePropertyChanged(() => Y);
            RaisePropertyChanged(() => Width);
            RaisePropertyChanged(() => Height);
            RaiseSelectionChanged();
            RaisePropertyChanged(() => IsSelected);
        }

        public TemplateImageViewModel Copy()
        {
            var viewModel = new TemplateImageViewModel(X, Y, Width, Height, Id, _parentWidth, _parentHeight)
                            {
                                _isSelected = IsSelected
                            };

            return viewModel;
        }

        public void CopyTo(TemplateImageViewModel to)
        {
            to._parentHeight = _parentHeight;
            to._parentWidth = _parentWidth;
            to._x = X;
            to._y = Y;
            to._width = Width;
            to._height = Height;
            to._isSelected = IsSelected;
            to.Id = Id;

            to.UpdateProperties();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value)
                    return;

                PushState();
                _isSelected = value;
                RaiseSelectionChanged();
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public event Action<ISelectable> SelectionChanged;

        public void SetSelected(bool status)
        {
            _isSelected = status;
            RaiseSelectionChanged();
            RaisePropertyChanged(() => IsSelected);
        }


        public void Resize(double deltaX, double deltaY, double offsetX, double offsetY)
        {
            X += (offsetX / _parentWidth).TwoDigits();
            Y += (offsetY / _parentHeight).TwoDigits();

            Width += (deltaX / _parentWidth).TwoDigits();
            Height += (deltaY / _parentHeight).TwoDigits();
        }

        public Type DataType { get { return typeof(TemplateImageViewModel); } }

        public void Update(double x, double y)
        {
            X = (x / _parentWidth).TwoDigits() - Width/2;
            Y = (y / _parentHeight).TwoDigits() - Height/2;
        }
    }

    public class TemplateImageViewModelExt : TemplateImageViewModel
    {
        private int _z;
        private byte[] _image;

        public TemplateImageViewModelExt() : base(0, 0)
        {
            
        }

        public TemplateImageViewModelExt(double x, double y, double width, double height, int id, double parentWidth, double parentHeight)
            : base(x, y, width, height, id, parentWidth, parentHeight)
        {
        }

        public TemplateImageViewModelExt(double parentWidth, double parentHeight) : base(parentWidth, parentHeight)
        {
        }

        public int Z
        {
            get { return _z; }
            set
            {
                if (_z == value)
                    return;

                _z = value;
                RaisePropertyChanged();
            }
        }

        public byte[] Image
        {
            get { return _image; }
            set { _image = value; }
        }
    }
}
