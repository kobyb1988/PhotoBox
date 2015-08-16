using System;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.CommonViewModels.Behaviors
{
    public class ShowWindowEventArgs : EventArgs
    {
        public BaseViewModel Context { get; private set; }

        public bool IsDialog { get; private set; }

        public ShowWindowEventArgs(BaseViewModel context, bool isDialog)
        {
            Context = context;
            IsDialog = isDialog;
        }
    }
}