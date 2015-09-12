using System;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using SandBox;

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

            double deltaX = 0;
            double deltaY = 0;
            double offsetX = 0;
            double offsetY = 0;

            if ((ResizeDirection & Direction.Right) == Direction.Right)
            {
                deltaX = dragDeltaEventArgs.HorizontalChange;
            }

            if ((ResizeDirection & Direction.Left) == Direction.Left)
            {
                offsetX = dragDeltaEventArgs.HorizontalChange;

                deltaX = -dragDeltaEventArgs.HorizontalChange;
            }

            if ((ResizeDirection & Direction.Bottom) == Direction.Bottom)
            {
                deltaY = dragDeltaEventArgs.VerticalChange;
            }

            if ((ResizeDirection & Direction.Top) == Direction.Top)
            {
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
