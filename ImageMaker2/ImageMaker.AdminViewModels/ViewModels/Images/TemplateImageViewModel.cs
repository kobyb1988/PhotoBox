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
        private int _index;

        public TemplateImageViewModel(double x, double y, double width, double height, int id, double parentWidth, double parentHeight) 
            : this(parentWidth, parentHeight)
        {
            Id = id;
            _x = x;
            _y = y;
            _width = width;
            //_height = height;
            _height = GetCorrectHeight(_width);
        }

        public TemplateImageViewModel(double parentWidth, double parentHeight)
        {
            _parentHeight = parentHeight;
            _parentWidth = parentWidth;
            _x = 0;
            _y = 0;
            _width = 0.1;
            //_height = 0.1;
            _height = GetCorrectHeight(_width);
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

        public int Index
        {
            get { return _index; }
            set
            {
                if (_index == value)
                    return;

                _index = value;
                RaisePropertyChanged();
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
            //RaiseSelectionChanged();
            RaisePropertyChanged(() => IsSelected);
        }


        public void Resize(double deltaX, double deltaY, double offsetX, double offsetY)
        {
            double testX = X + (offsetX / _parentWidth).ThreeDigits();
            double testY = Y + (offsetY / _parentHeight).ThreeDigits();
            double testW = Width + (deltaX / _parentWidth).ThreeDigits();
            double testH = Height + (deltaY / _parentHeight).ThreeDigits();

            if (testX < 0 || (testX + testW) > 1 || testY < 0 || (testY + testH) > 1)
                return;

            X = testX;
            Y = testY;

            Width = testW;
            //Height = testH;
            Height = GetCorrectHeight(testW);
        }

        public Type DataType { get { return typeof(TemplateImageViewModel); } }

        public void Update(double x, double y)
        {
            double testX = (x / _parentWidth).ThreeDigits() - (Width / 2).ThreeDigits();
            double testY = (y / _parentHeight).ThreeDigits() - (Height / 2).ThreeDigits();
            if (testX < 0 || (testX + Width) > 1 || testY < 0 || (testY + Height) > 1)
                return;

            X = testX;
            Y = testY;
        }

        private double GetCorrectHeight(double width)
        {
            //default camera resolution is 1056w x 704h
            return width/1056.0*704;
        }
    }
}
