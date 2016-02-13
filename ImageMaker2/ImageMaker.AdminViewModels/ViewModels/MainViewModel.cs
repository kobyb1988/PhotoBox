using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.AdminViewModels.Services;
using ImageMaker.CommonViewModels.Behaviors;
using ImageMaker.CommonViewModels.Messenger;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using NLog;
using System.Windows.Threading;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class MainViewModel : BaseViewModel, ICloseable, IWindowContainer
    {
        private readonly SessionService _sessionService;
        private readonly CommunicationManager _communicationManager;
        private readonly DispatcherTimer _timer;

        public MainViewModel(
            IViewModelNavigator navigator,
            IMessenger messenger,
            SessionService sessionService,
            CommunicationManager communicationManager)
        {
            _sessionService = sessionService;
            _communicationManager = communicationManager;
            messenger.Register<ShowChildWindowMessage>(this, RaiseShowWindow);

            messenger.Register<WindowStateMessage>(this, state => RaiseStateChanged(state.State));

            messenger.Register<ContentChangedMessage>(this, OnContentChanged);
            //navigator.NavigateForward<WelcomeViewModel>(null); //temporary
            navigator.NavigateForward<PasswordPromptViewModel>(null);


            messenger.Register<CommandMessage>(this, OnOpenCommand);
            messenger.Register<CloseCommandMessage>(this, OnCloseCommand);
            communicationManager.Connect();
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 15);
            _timer.IsEnabled = true;
            _timer.Tick += SendPing;
            _timer.Start();
        }
        private void SendPing(object sender, EventArgs e)
        {
            _communicationManager.Ping();
        }
        private void OnCloseCommand(CloseCommandMessage command)
        {
            _process = null;
            RaiseRequestClose(WindowState.Visible);
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
            RequestWindowVisibilityChanged?.Invoke(state);
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
        private RelayCommand _showMainCommand;

        public BaseViewModel CurrentContent
        {
            get { return _currentContent; }
            set
            {
                _currentContent = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand ShowMainCommand
        {
            get { return _showMainCommand ?? (_showMainCommand = new RelayCommand(ShowMain)); }
        }

        private void ShowMain()
        {
            StartMain();
        }

        private const string CMain = @"ImageMaker.View.exe";


        private Process _process;
        private void StartMain()
        {
            RaiseRequestClose(WindowState.Hidden);
            //Если процесс завершился аварийно то он останется в VM
            if (_process != null && Process.GetProcessesByName(CMain).Length > 0)
            {
                _communicationManager.SendHideCommand();
                return;
            }

            _sessionService.StartSession();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _process = Process.Start(new ProcessStartInfo(CMain));
                    _process.ErrorDataReceived += ErorHandle;
                    _process.WaitForExit();
                }
                catch (Exception ex)
                {

                    LogManager.GetCurrentClassLogger().Error(ex);
                }
                
            });
        }

        private void ErorHandle(object sender, DataReceivedEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Error(e.Data);
        }

        public event EventHandler<bool> StateChanged;
        public event Action<WindowState> RequestWindowVisibilityChanged;

        public void OnClose()
        {
            _communicationManager.SendCloseCommand();
            _timer.Tick -= SendPing;
            _timer.Stop();
            _communicationManager.SendHideCommand();
            Thread.Sleep(3000); //todo to wait for main window to close, find the better way
        }

        public event EventHandler<ShowWindowEventArgs> ShowWindow;
    }
}
