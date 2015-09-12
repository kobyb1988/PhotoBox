using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using ImageMaker.Common.Annotations;
using ImageMaker.CommonViewModels.DragDrop;

namespace ImageMaker.CommonView.Behaviors.DragDrop
{
    public class DragBehavior : Behavior<FrameworkElement>
    {

        public FrameworkElement RelativeElement
        {
            get { return (FrameworkElement)GetValue(RelativeElementProperty); }
            set { SetValue(RelativeElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RelativeElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RelativeElementProperty =
            DependencyProperty.Register("RelativeElement", typeof(FrameworkElement), typeof(DragBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseLeftButtonDown;
            Window.GetWindow(this.AssociatedObject).MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
            this.AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            this.AssociatedObject.PreviewGiveFeedback += AssociatedObjectOnGiveFeedback;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            Point currentPosition = mouseEventArgs.GetPosition(RelativeElement);

            //Point currentPosition = mouseEventArgs.GetPosition(Window.GetWindow(this.AssociatedObject));
            if (!_isMousePressed || !this.AssociatedObject.IsMouseOver)
                return;

            if (Math.Abs(_originalPosition.X - currentPosition.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(_originalPosition.Y - currentPosition.Y) < SystemParameters.MinimumVerticalDragDistance)
                return;

            IDragable context = this.AssociatedObject.DataContext as IDragable;
            if (context == null)
                return;


            Debug.WriteLine("Mouse leave");
            _adorner = new DefaultAdorner(this.AssociatedObject, new Point(0, 0), RelativeElement);

            DataObject data = new DataObject();
            data.SetData(context.DataType, context);
            System.Windows.DragDrop.DoDragDrop(this.AssociatedObject, data, DragDropEffects.Move);

            if (_adorner != null)
            {
                _adorner.Destroy();
                _adorner = null;
            }

            _isMousePressed = false;
        }


        private void AssociatedObjectOnGiveFeedback(object sender, GiveFeedbackEventArgs giveFeedbackEventArgs)
        {
            Debug.WriteLine("feedback");

            Point mouseCoordinates = Extensions.GetMouseCoordinates();
            var box = RelativeElement as Viewbox;
            double factor = box.GetScaleFactor();

            //Debug.WriteLine("initial x: {0}; y: {1}", mouseCoordinates.X, mouseCoordinates.Y);
            Point mousePosition = this.AssociatedObject.PointFromScreen(mouseCoordinates);

            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.AssociatedObject);
            var relative = RelativeElement.TranslatePoint(mousePosition, layer);
            Debug.WriteLine("relative to layer x: {0}; y: {1}", relative.X, relative.Y);
            var towindow = RelativeElement.TranslatePoint(mousePosition, Window.GetWindow(this.AssociatedObject));
            Debug.WriteLine("relative to layer x: {0}; y: {1}", towindow.X, towindow.Y);
            //Point mousePosition = Window.GetWindow(this.AssociatedObject).PointFromScreen(Extensions.GetMouseCoordinates());
            if (_adorner != null)
            {
                _adorner.SetMousePosition(mousePosition, factor);
            }
        }

        private bool _isMousePressed;
        private DefaultAdorner _adorner;

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _isMousePressed = false;
            Debug.WriteLine("Mouse up");
        }

        private Point _originalPosition;

        private void AssociatedObjectOnMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _isMousePressed = true;

            _originalPosition = mouseButtonEventArgs.GetPosition(RelativeElement);

            //_originalPosition = mouseButtonEventArgs.GetPosition(Window.GetWindow(this.AssociatedObject));

            Debug.WriteLine("Mouse down");
        }
    }

    public class DefaultAdorner : Adorner
    {
        private FrameworkElement _child;
        private Point _adornerOrigin;
        private Point _adornerOffset;
        private readonly FrameworkElement _relative;

        /// <summary>
        /// Create an adorner.
        /// The created adorner must then be added to the AdornerLayer.
        /// </summary>
        /// <param name="adornedElement">Element whose AdornerLayer will be use for displaying the adorner</param>
        /// <param name="adornerElement">Element used as adorner</param>
        /// <param name="adornerOrigin">Origin offset within the adorner</param>
        /// <param name="opacity">Adorner's opacity</param>
        public DefaultAdorner(UIElement adornedElement, Point origin, FrameworkElement relative)
            : base(adornedElement)
        {
            Rectangle rect = new Rectangle();
            rect.Width = adornedElement.RenderSize.Width;
            rect.Height = adornedElement.RenderSize.Height;

            VisualBrush visualBrush = new VisualBrush(adornedElement);
            visualBrush.Opacity = 0.5;
            visualBrush.Stretch = Stretch.None;
            rect.Fill = visualBrush;

            
            this._child = rect;

            this._adornerOrigin = new Point(0, 0);
                
            this._adornerOffset = origin;
            _relative = relative;
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(adornedElement);

            Adorner[] adorners = layer.GetAdorners(adornedElement);
            if (adorners != null)
            {
                Array.ForEach(adorners, layer.Remove);   
            }
            
            layer.Add(this);
            InvalidateVisual();
        }

        /// <summary>
        /// Set the position of and redraw the adorner.
        /// Call when the mouse cursor position changes.
        /// </summary>
        /// <param name="position">Adorner's new position relative to AdornerLayer origin</param>
        public void SetMousePosition(Point position, double factor)
        {
            Debug.WriteLine("x: {0}; y: {1}", position.X, position.Y);
            Debug.WriteLine("x: {0}; y: {1}", position.X, position.Y);
            this._adornerOffset.X = (position.X * factor) - this._adornerOrigin.X - (_child.Width * factor) / 2;
            this._adornerOffset.Y = (position.Y * factor) - this._adornerOrigin.Y - (_child.Height * factor) / 2;

            UpdatePosition();
        }

        private void UpdatePosition()
        {
            AdornerLayer.GetAdornerLayer(AdornedElement).Update();
        }

        protected override int VisualChildrenCount { get { return 1; } }

        protected override Visual GetVisualChild(int index)
        {
            System.Diagnostics.Debug.Assert(index == 0, "Index must be 0, there's only one child");
            return this._child;
        }

        protected override Size MeasureOverride(Size finalSize)
        {
            this._child.Measure(finalSize);
            return this._child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this._child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            Debug.WriteLine(transform);

            GeneralTransformGroup newTransform = new GeneralTransformGroup();
            
            newTransform.Children.Add(base.GetDesiredTransform(transform));
            newTransform.Children.Add(new TranslateTransform(this._adornerOffset.X, this._adornerOffset.Y));
            return newTransform;
        }

        public void Destroy()
        {
            AdornerLayer.GetAdornerLayer(AdornedElement).Remove(this);
        }
    }


    public static class ViewBoxExtensions
    {
        public static double GetScaleFactor(this Viewbox viewbox)
        {
            if (viewbox.Child == null ||
                (viewbox.Child is FrameworkElement) == false)
            {
                return double.NaN;
            }

            FrameworkElement child = viewbox.Child as FrameworkElement;
            return viewbox.ActualWidth / child.ActualWidth;
        }
    }
}
