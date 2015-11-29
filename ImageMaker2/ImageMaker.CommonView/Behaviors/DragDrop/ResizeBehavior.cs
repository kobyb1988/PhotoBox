using System;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using ImageMaker.CommonViewModels.DragDrop;

namespace ImageMaker.CommonView.Behaviors.DragDrop
{
    public class ResizeBehavior : Behavior<Thumb>
    {
        public Direction ResizeDirection { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.DragDelta += AssociatedObjectOnDragDelta;
        }

        private void AssociatedObjectOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
        {
            IResizable resizable = this.AssociatedObject.DataContext as IResizable;
            if (resizable == null)
                return;
            if (!Debugger.IsAttached)
                Debugger.Launch();

            double deltaX = 0;
            double deltaY = 0;
            double offsetX = 0;
            double offsetY = 0;

            if ((ResizeDirection & Direction.Right) == Direction.Right || (ResizeDirection & Direction.Left) == Direction.Left)
            {
                deltaX = dragDeltaEventArgs.HorizontalChange;
                offsetX = dragDeltaEventArgs.HorizontalChange;

                deltaX = -dragDeltaEventArgs.HorizontalChange;
                deltaY = dragDeltaEventArgs.HorizontalChange;
                offsetY = dragDeltaEventArgs.HorizontalChange;

                deltaY = -dragDeltaEventArgs.HorizontalChange;
            }

            if ((ResizeDirection & Direction.Bottom) == Direction.Bottom || (ResizeDirection & Direction.Top) == Direction.Top)
            {
                deltaX = dragDeltaEventArgs.VerticalChange;
                offsetX = dragDeltaEventArgs.VerticalChange;

                deltaX = -dragDeltaEventArgs.VerticalChange;
                deltaY = dragDeltaEventArgs.VerticalChange;
                offsetY = dragDeltaEventArgs.VerticalChange;

                deltaY = -dragDeltaEventArgs.VerticalChange;
            }
            
            resizable.Resize(deltaX, deltaY, offsetX, offsetY);
        }
    }

    [Flags]
    public enum Direction
    {
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }
}
