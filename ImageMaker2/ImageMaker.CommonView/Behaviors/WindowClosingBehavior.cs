using System;
using System.Windows;
using System.Windows.Interactivity;
using ImageMaker.CommonView.Helpers;
using ImageMaker.CommonViewModels.Behaviors;

namespace ImageMaker.CommonView.Behaviors
{
    public class WindowClosingBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += (sender, args) =>
            {
                ICloseable closeable = this.AssociatedObject.DataContext as ICloseable;
                if (closeable != null)
                {
                    closeable.StateChanged += (obj, state) => Application.Current.Dispatcher.BeginInvoke(new Action(() => this.AssociatedObject.SetWindowCloseStatus(state)));

                    closeable.RequestClose += () => this.AssociatedObject.Close();
                }
            };
        }
    }
}
