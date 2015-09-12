using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using GalaSoft.MvvmLight;

namespace SandBox.ViewModels
{
    public class DragViewModel : ViewModelBase, IDragable, IResizable
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        const double CTolerance = 0.00001;

        public DragViewModel()
        {
            _width = 100;
            _height = 100;
            _x = 300;
            _y = 100;
        }

        public Type DataType { get { return typeof (DragViewModel); } }
        public void Update(double x, double y)
        {
            X = x - Width / 2;
            Y = y - Height / 2;
        }

        public double Width
        {
            get { return _width; }
            set
            {
                if (Math.Abs(_width - value) < CTolerance)
                    return;

                _width = value;
                RaisePropertyChanged();
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                if (Math.Abs(_height - value) < CTolerance)
                    return;

                _height = value;
                RaisePropertyChanged();
            }
        }

        public double X
        {
            get { return _x; }
            set
            {
                if (Math.Abs(_x - value) < CTolerance)
                    return;

                _x = value;
                RaisePropertyChanged();
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                if (Math.Abs(_y - value) < CTolerance)
                    return;

                _y = value;
                RaisePropertyChanged();
            }
        }

        public void Resize(double deltaX, double deltaY, double offsetX, double offsetY)
        {
            X += offsetX;
            Y += offsetY;

            Width += deltaX;
            Height += deltaY;
        }
    }

    public class DropViewModel : ViewModelBase, IDropable
    {
        public Type DataType { get { return typeof (DragViewModel); } }

        public void Drop(object data)
        {
            Debug.WriteLine("Drop");
        }
    }
}
