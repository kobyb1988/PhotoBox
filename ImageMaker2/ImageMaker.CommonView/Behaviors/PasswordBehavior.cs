using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using ImageMaker.CommonViewModels.Behaviors;

namespace ImageMaker.CommonView.Behaviors
{
    public class PasswordBehavior : Behavior<PasswordBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += delegate
                                            {
                                                IPassword password = this.AssociatedObject.DataContext as IPassword;
                                                if (password != null)
                                                {
                                                    this.AssociatedObject.PasswordChanged += (sender, args) =>
                                                    {
                                                        password.Password = this.AssociatedObject.Password;
                                                    };
                                                }
                                            };
        }
    }
}
