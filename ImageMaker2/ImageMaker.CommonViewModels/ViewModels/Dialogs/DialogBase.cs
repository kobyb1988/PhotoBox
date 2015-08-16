using System;
using ImageMaker.CommonViewModels.Behaviors;

namespace ImageMaker.CommonViewModels.ViewModels.Dialogs
{
    public abstract class DialogBase : BaseViewModel, ICloseable
    {
        public event EventHandler<bool> StateChanged;
        public event Action RequestClose;

        protected void Close()
        {
            var handler = RequestClose;
            if (handler != null)
                handler();
        }

        public abstract string Title { get; }
    }
}