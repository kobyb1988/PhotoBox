using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Async;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.PatternProcessing;
using ImageMaker.PatternProcessing.Dto;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.SDKData;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;
using NLog;

namespace ImageMaker.ViewModels.ViewModels
{
    public class CameraViewModel : BaseViewModel
    {
        private const int CDefWidth = 2048;
        private const int CDefHeight = 1536;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private CameraSettingsDto _settings;
        private readonly SettingsProvider _settingsProvider;
        private readonly IDialogService _dialogService;
        private readonly IViewModelNavigator _navigator;
        private readonly CompositionModelProcessor _imageProcessor;
        private int _width;
        private int _height;
        private int _imageNumber;

        private RelayCommand _goBackCommand;
        private RelayCommand _openSessionCommand;
        private RelayCommand _closeSessionCommand;
        private RelayCommand _refreshCameraCommand;
        private RelayCommand _startLiveViewCommand;
        private IAsyncCommand _takePictureCommand;
        private RelayCommand<uint> _setFocusCommand;

        private byte[] _liveViewImageStream;

        private bool _sessionOpened;
        private bool _takingPicture;
        private bool _capturing;
        private bool _isLiveViewOn;
        private int _counter;

        static AutoResetEvent _cameraStreamSynchronize;
        public CameraViewModel(
            SettingsProvider settingsProvider,
            IDialogService dialogService,
            IViewModelNavigator navigator,
            CompositionModelProcessor imageProcessor
            )
        {
            _settingsProvider = settingsProvider;
            _dialogService = dialogService;
            _navigator = navigator;
            _imageProcessor = imageProcessor;

            _width = CDefWidth;
            _height = CDefHeight;
        }

        public override void Initialize()
        {
            _logger.Trace("Инициализация");
            _imageProcessor.TimerElapsed += ImageProcessorOnTimerElapsed;
            _imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged += ImageProcessorOnStreamChanged;
            _imageProcessor.ImageNumberChanged += ImageProcessorOnImageNumberChanged;

            Capturing = false;

            _imageProcessor.InitializeProcessor();
            OpenSession();
            if (!_sessionOpened)
                return;

            _settings = _settingsProvider.GetCameraSettings();

            if (_settings != null)
            {
                _logger.Trace("Применение настроек для камеры");

                _imageProcessor.SetSetting((uint)PropertyId.AEMode, (uint)_settings.SelectedAeMode);
                _imageProcessor.SetSetting((uint)PropertyId.WhiteBalance, (uint)_settings.SelectedWhiteBalance);
                _imageProcessor.SetSetting((uint)PropertyId.Av, (uint)_settings.SelectedAvValue);
                _imageProcessor.SetSetting((uint)PropertyId.ExposureCompensation, (uint)_settings.SelectedCompensation);
                _imageProcessor.SetSetting((uint)PropertyId.ISOSpeed, (uint)_settings.SelectedIsoSensitivity);
                _imageProcessor.SetSetting((uint)PropertyId.Tv, (uint)_settings.SelectedShutterSpeed);
            }
            _cameraStreamSynchronize = new AutoResetEvent(false);
            _logger.Trace("Запуск LiveView");
            StartLiveView();
            var cancellTokenSource = new CancellationTokenSource();
            if (TakePictureCommand.CanExecute(cancellTokenSource.Token))
            {
                try
                {
                    TakePictureCommand.Execute(cancellTokenSource.Token);
                }
                catch (Exception ex)
                {
                    //_dialogService.ShowInfo("Упс... С камерой возникли неполадки. Приносим свои извинения. =(");
                    _logger.Error(ex, "Ошибка при инициализации камеры");
                    GoBack();
                }
            }
            else
            {
                //_dialogService.ShowInfo("Упс... С камерой возникли неполадки. Приносим свои извинения. =(");
                _logger.Trace("TakePictureCommand.CanExecute вернул false");
                GoBack();
            }
        }

        private void ImageProcessorOnImageNumberChanged(object sender, int newValue)
        {
            ImageNumber = newValue;
        }

        public override void Dispose()
        {
            _logger.Trace("Очистка");

            _imageProcessor.TimerElapsed -= ImageProcessorOnTimerElapsed;
            _imageProcessor.CameraErrorEvent -= ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged -= ImageProcessorOnStreamChanged;
            _imageProcessor.ImageNumberChanged -= ImageProcessorOnImageNumberChanged;

            _sessionOpened = false;
            TakingPicture = false;
            _isLiveViewOn = false;
            _imageProcessor.Dispose();
        }

        private void ImageProcessorOnTimerElapsed(object sender, int tick)
        {
            Counter = tick;

            //костыль. камера падает, если возвращаться назад во время фотографирования
            Capturing = Counter == 0;
        }

        private void ImageProcessorOnCameraErrorEvent(object sender, CameraEventBase cameraErrorInfo)
        {
            switch (cameraErrorInfo.EventType)
            {
                case CameraEventType.Shutdown:
                    _logger.Trace("Пришло событие CameraEventType.Shutdown");

                    TakingPicture = false;
                    _dialogService.ShowInfo(cameraErrorInfo.Message);
                    Dispose();
                    UpdateCommands();
                    break;
                case CameraEventType.Error:
                    var oldCameraStatus = TakingPicture;
                    TakingPicture = false;
                    SetWindowStatus(true);
                    UpdateCommands();

                    var ev = cameraErrorInfo as ErrorEvent;
                    if (ev != null && ev.ErrorCode == ReturnValue.TakePictureAutoFocusNG)
                    {
                        _dialogService.ShowInfo("Не удалось сфокусироваться. Пожалуйста, повторите попытку.");
                        Dispose();
                        Initialize();
                    }
                    if (ev != null && ev.ErrorCode == ReturnValue.NotSupported)
                    {
                        _logger.Trace("Пришло событие ошибки CameraEventType.Error, Type: NotSupported");
                        //не убирать. пропадёт отсчёт
                        //camera is still in previous state, just some setting parameter is not supported
                        TakingPicture = oldCameraStatus;
                        return;
                    }
                    else
                    {
                        _logger.Error($"Пришло событие ошибки CameraEventType.Error, Type: {ev?.ErrorCode}");
                        // _dialogService.ShowInfo("Упс... Что-то пошло не так =(");//TODO разобраться как перезапустить камеру
                    }
                    GoBack();

                    break;
            }
        }

        private void ImageProcessorOnStreamChanged(object sender, ImageDto image)
        {
            Width = image.Width;
            Height = image.Height;
            LiveViewImageStream = image.ImageData;
            if (LiveViewImageStream.Length > 0)
                _cameraStreamSynchronize.Set();
        }

        private async Task<CompositionProcessingResult> TakePicture(CancellationToken token)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    _logger.Trace("Начало фотографирования");
                    TakingPicture = true;
                    UpdateCommands();

                    token.ThrowIfCancellationRequested();

                    //_imageProcessor.ImageChanged -= ImageProcessorOnStreamChanged;
                    _logger.Trace("Начало фотографирования:Синхронизация LiveView");
                    _cameraStreamSynchronize.WaitOne();
                    _logger.Trace("Начало фотографирования:Синхронизация закончена");

                    var copyLiveViewStream = LiveViewImageStream;
                    await Task.Delay(TimeSpan.FromSeconds(2), token);
                    var stream = await _imageProcessor.TakePictureAsync(copyLiveViewStream,
                        _settings, token);

                    token.ThrowIfCancellationRequested();
                    _logger.Trace("Конец фотографирования");
                    //TakingPicture = false;
                    UpdateCommands();

                    SetWindowStatus(true);

                    _navigator.NavigateForward<CameraResultViewModel>(this, stream);
                    return stream;
                }
                catch (OperationCanceledException e)
                {
                    return new CompositionProcessingResult(null, LiveViewImageStream);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Ошибка фотографирования");
                    throw;
                }

            }, token);
        }

        private void StartLiveView()
        {
            _imageProcessor.StartLiveView();
            _isLiveViewOn = true;
            UpdateCommands();
        }

        private void RefreshCamera()
        {
            _imageProcessor.RefreshCamera();
        }

        private void CloseSession()
        {
            _imageProcessor.CloseSession();
            _sessionOpened = false;
            UpdateCommands();
        }

        private void OpenSession()
        {
            bool result = _imageProcessor.OpenSession();
            if (!result)
            {
                _dialogService.ShowInfo("Камера отсутствует или не готова");
                GoBack();
                return;
            }

            _sessionOpened = true;
            UpdateCommands();
        }

        private void UpdateCommands()
        {
            OpenSessionCommand.RaiseCanExecuteChanged();
            CloseSessionCommand.RaiseCanExecuteChanged();
            //TakePictureCommand.RaiseCanExecuteChanged();//TODO перепроверить
            StartLiveViewCommand.RaiseCanExecuteChanged();
            RefreshCameraCommand.RaiseCanExecuteChanged();

            SetWindowStatus(!_sessionOpened);
        }

        private void SetWindowStatus(bool status)
        {
            _dialogService.SetWindowCloseState(status);
        }

        private void GoBack()
        {
            SetWindowStatus(true);

            _cameraStreamSynchronize.Do(x => x.Set());
            var takePictireCmd = (AsyncCommand<Task<CompositionProcessingResult>>)TakePictureCommand;
            if (takePictireCmd.CancelCommand.CanExecute(null))
                takePictireCmd.CancelCommand.Execute(null);

            Dispose();
            _navigator.NavigateBack(this);
        }


        public int Counter
        {
            get { return _counter; }
            set { Set(() => Counter, ref _counter, value); }
        }

        public bool Capturing
        {
            get { return _capturing; }
            set { Set(() => Capturing, ref _capturing, value); }
        }

        public bool TakingPicture
        {
            get { return _takingPicture; }
            set
            {
                if (_takingPicture == value)
                    return;

                _takingPicture = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => NotTakingPicture);
            }
        }

        public bool NotTakingPicture => !TakingPicture;

        public int Width
        {
            get { return _width; }
            set
            {
                Set(() => Width, ref _width, value);
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                Set(() => Height, ref _height, value);
            }
        }

        public int ImageNumber
        {
            get { return _imageNumber; }
            set { Set(() => ImageNumber, ref _imageNumber, value); }
        }

        public byte[] LiveViewImageStream
        {
            get { return _liveViewImageStream; }
            set
            {
                Set(() => LiveViewImageStream, ref _liveViewImageStream, value);
            }
        }

        public IAsyncCommand TakePictureCommand
        {
            get
            {
                return _takePictureCommand ??
                       (_takePictureCommand =
                           AsyncCommand.Create<Task<CompositionProcessingResult>>(t => TakePicture(t),
                               () => _sessionOpened && !TakingPicture));
            }
        }

        #region unused


        private void SetFocus(uint focus)
        {
            _imageProcessor.SetFocus(focus);
        }

        private IList<uint> _focusPoints;


        public IList<uint> FocusPoints
            => _focusPoints ?? (_focusPoints = Enum.GetValues(typeof (LiveViewDriveLens)).OfType<uint>().ToList());

        public RelayCommand<uint> SetFocusCommand
        {
            get
            {
                return _setFocusCommand ?? (_setFocusCommand = new RelayCommand<uint>(SetFocus,
                    (x) => _sessionOpened && !TakingPicture && _isLiveViewOn));
            }
        }

        public RelayCommand GoBackCommand
            => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack, CanGoBack));

        private bool CanGoBack()
        {
            return !Capturing;
        }

        public RelayCommand OpenSessionCommand
        {
            get { return _openSessionCommand ?? (_openSessionCommand = new RelayCommand(OpenSession, () => true)); } //todo
        }

        public RelayCommand CloseSessionCommand
        {
            get { return _closeSessionCommand ?? (_closeSessionCommand = new RelayCommand(CloseSession, () => _sessionOpened && !TakingPicture)); }
        }

        public RelayCommand RefreshCameraCommand
        {
            get { return _refreshCameraCommand ?? (_refreshCameraCommand = new RelayCommand(RefreshCamera, () => _sessionOpened && !TakingPicture)); }
        }

        public RelayCommand StartLiveViewCommand
        {
            get { return _startLiveViewCommand ?? (_startLiveViewCommand = new RelayCommand(StartLiveView, () => _sessionOpened && !TakingPicture)); }
        }

        #endregion
    }
}
