using System;
using System.Diagnostics;
using ImageMaker.CommonViewModels.Behaviors;
using ImageMaker.CommonViewModels.Messenger;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel, ICloseable, IWindowContainer
    {
        public MainViewModel(
            IChildrenViewModelsFactory childFactory,
            IViewModelNavigator navigator,
            IMessenger messenger)
        {
            messenger.Register<ShowChildWindowMessage>(this, RaiseShowWindow);

            messenger.Register<WindowStateMessage>(this, state => RaiseStateChanged(state.State));

            messenger.Register<ContentChangedMessage>(this, OnContentChanged);
            navigator.NavigateForward(childFactory.GetChild<WelcomeViewModel>(null));
        }

        private void OnContentChanged(ContentChangedMessage message)
        {
            if (CurrentContent != null)
                CurrentContent.Dispose();

            CurrentContent = message.Content;
            if (CurrentContent != null)
                CurrentContent.Initialize();
        }

        private void RaiseStateChanged(bool state)
        {
            var handler = StateChanged;
            if (handler != null)
                handler(this, state);
        }

        private void RaiseRequestClose()
        {
            var handler = RequestClose;
            if (handler != null)
                handler();
        }

        private void RaiseShowWindow(ShowChildWindowMessage message)
        {
            var handler = ShowWindow;
            if (handler != null)
                handler(this, new ShowWindowEventArgs(message.Content, message.IsDialog));
        }


        public override void Dispose()
        {
            Debug.WriteLine("Disposing MainViewModel...");
            if (CurrentContent != null)
                CurrentContent.Dispose();
        }

        private BaseViewModel _currentContent;

        public BaseViewModel CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler<bool> StateChanged;
        public event Action RequestClose;

        public event EventHandler<ShowWindowEventArgs> ShowWindow;
    }
}
