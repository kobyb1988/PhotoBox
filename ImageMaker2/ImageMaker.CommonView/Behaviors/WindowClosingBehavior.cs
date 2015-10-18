using System;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Interactivity;
using ImageMaker.CommonView.Helpers;
using ImageMaker.CommonViewModels.Behaviors;
using WindowState = ImageMaker.CommonViewModels.Behaviors.WindowState;

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
                    this.AssociatedObject.Closing += (o, eventArgs) => closeable.OnClose();

                    closeable.RequestWindowVisibilityChanged += (state) =>
                                              {
                                                  Application.Current.Dispatcher.InvokeAsync(() =>
                                                                                             {
                                                                                                 switch (state)
                                                                                                 {
                                                                                                     case WindowState.Closed:
                                                                                                         this.AssociatedObject.Close();
                                                                                                         break;
                                                                                                     case WindowState.Hidden:
                                                                                                         this.AssociatedObject.Hide();
                                                                                                         break;
                                                                                                     case WindowState.Visible:
                                                                                                         this.AssociatedObject.Show();
                                                                                                         break;
                                                                                                     default:
                                                                                                         throw new ArgumentOutOfRangeException("state");
                                                                                                 }
                                                                                             });
                                                  ;
                                              };
                }
            };
        }
    }
}
