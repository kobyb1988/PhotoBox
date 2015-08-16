using System;
using System.Linq.Expressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;

namespace ImageMaker.CommonViewModels.ViewModels
{
    public class BaseViewModel : ViewModelBase, IDisposable
    {
        static BaseViewModel()
        {
            DispatcherHelper.Initialize();
        }

        public virtual void Dispose()
        {
        }

        public virtual void Initialize()
        {
            
        }

        protected virtual void UiInvoke(Action action)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(action);
        }
    }
}
