using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Behaviors;
using ImageMaker.CommonViewModels.Messenger;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.ViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel, ICloseable, IWindowContainer
    {
        private readonly CommunicationManager _communicationManager;

        public MainViewModel(
            IViewModelNavigator navigator, 
            IMessenger messenger,
            CommunicationManager communicationManager
            )
        {
            _communicationManager = communicationManager;
            messenger.Register<ShowChildWindowMessage>(this, RaiseShowWindow);
            
            messenger.Register<WindowStateMessage>(this, state => RaiseStateChanged(state.State));

            messenger.Register<ContentChangedMessage>(this, OnContentChanged);
            navigator.NavigateForward<WelcomeViewModel>(null);

            messenger.Register<CommandMessage>(this, OnOpenCommand);
            communicationManager.Connect();
        }

        private void OnOpenCommand(CommandMessage command)
        {
            RaiseRequestClose(WindowState.Visible);
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

        private void RaiseRequestClose(WindowState state)
        {
            var handler = RequestWindowVisibilityChanged;
            if (handler != null)
                handler(state);
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
        private RelayCommand _showAdminCommand;

        public BaseViewModel CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ShowAdminCommand
        {
            get { return _showAdminCommand ?? (_showAdminCommand = new RelayCommand(ShowAdmin)); }
        }

        private void ShowAdmin()
        {
            RaiseRequestClose(WindowState.Hidden);
            _communicationManager.SendHideCommand();
        }

        public event EventHandler<bool> StateChanged;
        public event Action<WindowState> RequestWindowVisibilityChanged;

        public event EventHandler<ShowWindowEventArgs> ShowWindow;
    }
}
