using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interactivity;

namespace SandBox
{
    public class DropBehavior : Behavior<FrameworkElement>
    {


        public IInputElement RelativeElement
        {
            get { return (IInputElement)GetValue(RelativeElementProperty); }
            set { SetValue(RelativeElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RelativeElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RelativeElementProperty =
            DependencyProperty.Register("RelativeElement", typeof(IInputElement), typeof(DropBehavior), new PropertyMetadata(null));

        

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.DragOver += AssociatedObjectOnDragOver;
            this.AssociatedObject.DragEnter += AssociatedObjectDragEnter;
            this.AssociatedObject.DragLeave += AssociatedObjectOnDragLeave;
            this.AssociatedObject.Drop += AssociatedObjectOnDrop;
        }

        private Type _dataType;

        private void AssociatedObjectOnDragLeave(object sender, DragEventArgs dragEventArgs)
        {
            dragEventArgs.Handled = true;
        }

        private void AssociatedObjectDragEnter(object sender, DragEventArgs e)
        {
            if (_dataType == null)
            {
                IDropable dropTarget = this.AssociatedObject.DataContext as IDropable;
                if (dropTarget != null)
                    _dataType = dropTarget.DataType;
            }

            e.Handled = true;
        }

        private void AssociatedObjectOnDrop(object sender, DragEventArgs dragEventArgs)
        {
            if (_dataType != null)
            {
                if (dragEventArgs.Data.GetDataPresent(_dataType))
                {
                    IDropable target = this.AssociatedObject.DataContext as IDropable;
                    if (target != null)
                    {
                        IDragable source = dragEventArgs.Data.GetData(_dataType) as IDragable;
                        if (source != null)
                        {
                            Debug.WriteLine("Drop");
                            Point currentPosition = dragEventArgs.GetPosition(RelativeElement);
                            source.Update(currentPosition.X, currentPosition.Y);    
                        }
                        
                        //target.Drop(dragEventArgs.Data.GetData(_dataType));
                    }
                }   
            }

            dragEventArgs.Handled = true;
        }

        private void AssociatedObjectOnDragOver(object sender, DragEventArgs dragEventArgs)
        {
            if (_dataType != null)
            {
                if (dragEventArgs.Data.GetDataPresent(_dataType))
                {
                    SetDragDropEffects(dragEventArgs);
                }
            }

            dragEventArgs.Handled = true;
        }

        private void SetDragDropEffects(DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(_dataType))
            {
                e.Effects = DragDropEffects.Move;
            }
        }
    }

    public static class Extensions
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }


        public static Point GetMouseCoordinates()
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                //return new Point(p.X, p.Y);
                Point wpfPoint = ConvertPixelsToUnits(p.X, p.Y);
                //System.Console.WriteLine(Convert.ToString(wpfPoint.X) + ";" + Convert.ToString(wpfPoint.Y));
                return wpfPoint;
            }

            return new Point(0, 0);
        }

        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        private static Point ConvertPixelsToUnits(int x, int y)
        {
            // get the system DPI
            IntPtr dDC = GetDC(IntPtr.Zero); // Get desktop DC
            int dpi = GetDeviceCaps(dDC, 88);
            bool rv = ReleaseDC(IntPtr.Zero, dDC);

            // WPF's physical unit size is calculated by taking the 
            // "Device-Independant Unit Size" (always 1/96)
            // and scaling it by the system DPI
            double physicalUnitSize = (1d / 96d) * (double)dpi;
            Point wpfUnits = new Point(physicalUnitSize * (double)x,
                physicalUnitSize * (double)y);

            return wpfUnits;
        }
    }
}
