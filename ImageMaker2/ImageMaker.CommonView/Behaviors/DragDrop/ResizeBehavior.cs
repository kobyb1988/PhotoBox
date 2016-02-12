﻿using System;
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
            var resizable = this.AssociatedObject.DataContext as IResizable;
            if (resizable == null)
                return;

            double deltaX = 0,
                deltaY = 0,
                offsetX = 0,
                offsetY = 0;

            if ((ResizeDirection & Direction.Left) == Direction.Left)
            {
                offsetX = dragDeltaEventArgs.HorizontalChange;
                offsetY = dragDeltaEventArgs.HorizontalChange;

                deltaX = -dragDeltaEventArgs.HorizontalChange;
                deltaY = -dragDeltaEventArgs.HorizontalChange;
            }
            if ((ResizeDirection & Direction.Right) == Direction.Right)
            {
                deltaX = dragDeltaEventArgs.HorizontalChange;
                deltaY = dragDeltaEventArgs.HorizontalChange;
            }

            if ((ResizeDirection & Direction.Top) == Direction.Top)
            {
                offsetX = dragDeltaEventArgs.VerticalChange;
                offsetY = dragDeltaEventArgs.VerticalChange;

                deltaX = -dragDeltaEventArgs.VerticalChange;
                deltaY = -dragDeltaEventArgs.VerticalChange;
            }
            if ((ResizeDirection & Direction.Bottom) == Direction.Bottom)
            {
                deltaX = dragDeltaEventArgs.VerticalChange;
                deltaY = dragDeltaEventArgs.VerticalChange;
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
