using System;
using System.Monads;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.PatternProcessing.Dto;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.SDKData;
using ImageMaker.SDKData.Enums;
using ImageMaker.SDKData.Events;
using NLog;

namespace ImageMaker.AdminViewModels.ViewModels.CamerSettingsExplorer
{
    public class CameraSettingsExplorerViewModel : BaseViewModel
    {
        #region Fields and Properties
        static AutoResetEvent _cameraStreamSynchronize;
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private readonly IDialogService _dialogService;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private RelayCommand _saveSettings;
        private RelayCommand _goBackCommand;
        private RelayCommand _takePhotoCommand;
        private RelayCommand _undoCommand;
        private RelayCommand _redoCommand;
        private bool _previewReady;
        private byte[] _liveViewImageStream;
        private readonly CompositionModelProcessor _imageProcessor;
        private int _width;
        private int _height;
        private bool _sessionOpened;
        private bool _takePhotoEnable;
        private int _testPhotoTimeEllapsed;
        private CameraSettingsViewModel _cameraSettings;

        public CameraSettingsViewModel CameraSettings => _cameraSettings;

        public int TestPhotoTimeEllapsed
        {
            get { return _testPhotoTimeEllapsed; }
            set { Set(() => TestPhotoTimeEllapsed, ref _testPhotoTimeEllapsed, value); }
        }

        public byte[] LiveViewImageStream
        {
            get { return _liveViewImageStream; }
            set
            {
                _liveViewImageStream = value;
                RaisePropertyChanged();
            }
        }

        public bool PreviewReady
        {
            get { return _previewReady; }
            set { Set(() => PreviewReady, ref _previewReady, value); }
        }

        public int Width
        {
            get { return _width; }
            set { Set(() => Width, ref _width, value); }
        }

        public int Height
        {
            get { return _height; }
            set { Set(() => Height, ref _height, value); }
        }
        #endregion


        public CameraSettingsExplorerViewModel(
            IViewModelNavigator navigator,
            SettingsProvider settingsProvider,
            CompositionModelProcessor imageProcessor,
            IMappingEngine mappingEngine, IDialogService dialogService)
        {
            _navigator = navigator;
            _settingsProvider = settingsProvider;
            _imageProcessor = imageProcessor;
            _mappingEngine = mappingEngine;
            _dialogService = dialogService;
        }

        #region Methods
        public override void Initialize()
        {
            _cameraSettings = new CameraSettingsViewModel(_imageProcessor, _settingsProvider);
            _imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged += ImageProcessorOnStreamChanged;
            _imageProcessor.CameraAddEvent += ImageProcessorCameraAddEvent;
            _imageProcessor.CameraRemoveEvent += ImageProcessorCameraRemoveEvent;
            _imageProcessor.InitializeProcessor();
            _cameraStreamSynchronize = new AutoResetEvent(false);
            TestPhotoTimeEllapsed = 0;
            _takePhotoEnable = PreviewReady;
            CommandManager.InvalidateRequerySuggested();

            OpenSession();
            if (!_sessionOpened)
                return;

            StartLiveView();
        }

        private void StartLiveView(bool setupDefaultSettings = true)
        {
            _imageProcessor.StartLiveView();
            CameraSettings.PrepareToLiveView(setupDefaultSettings);
        }

        private void CloseSession()
        {
            _imageProcessor.CloseSession();
            _sessionOpened = false;
        }

        private void OpenSession()
        {
            var result = _imageProcessor.OpenSession();
            if (!result)
            {
                PreviewReady = false;
                _sessionOpened = false;
            }
            else
            {
                PreviewReady = true;
                _sessionOpened = true;
            }
        }
        private void GoBack()
        {
            
            if (CameraSettings.CanUndo)
            {
                bool result = _dialogService.ShowConfirmationDialog("При переходе все изменения будут потеряны. Продолжить?");
                if (!result)
                    return;

                CameraSettings.ResetChanges() ;
            }

            CameraSettings.ClearChanges();
            Dispose();
            _cameraStreamSynchronize.Do(x => x.Set());
            _navigator.NavigateBack(this);
        }



        private void Save()
        {
            CameraSettings.ClearStackChanged();
            _settingsProvider.SaveCameraSettings(_mappingEngine.Map<CameraSettingsDto>(CameraSettings));
        }

        private async Task<byte[]> TakePhoto()
        {
            try
            {
                _takePhotoEnable = false;
                CommandManager.InvalidateRequerySuggested();

                _logger.Trace("Синхронизация потоков при фотографировании.");

                _cameraStreamSynchronize.WaitOne();
                _logger.Trace("Синхронизация потоков при фотографировании завершина.");

                var copyLiveViewStream = LiveViewImageStream;

                var stream = await _imageProcessor.TakeTestPictureAsync(copyLiveViewStream,
                    _mappingEngine.Map<CameraSettingsDto>(this));

                _logger.Trace("Свойство Livview обнговилось значением {0}.", stream.Length);

                LiveViewImageStream = stream;

                for (int j = 5; j >= 0; j--)
                {
                    TestPhotoTimeEllapsed = j;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                CameraSettings.SetCameraSettings();

                _logger.Trace("Запуск LiveView");
                StartLiveView(false);

                _takePhotoEnable = true;
                CommandManager.InvalidateRequerySuggested();
                return stream;
            }
            catch (Exception ex)
            {
                _takePhotoEnable = true;
                MessageBox.Show("Упс, сфотографироваться не удалось.=(");
                _logger.Error(ex, "Ошибка при фотографировании");
                return await Task.FromResult(new byte[] { });
            }

        }


        #region ImageProcessor Handling
        private void ImageProcessorOnCameraErrorEvent(object sender, CameraEventBase cameraErrorInfo)
        {
            switch (cameraErrorInfo.EventType)
            {
                case CameraEventType.Shutdown:
                    // Dispose();

                    break;
                case CameraEventType.Error:

                    var ev = cameraErrorInfo as ErrorEvent;
                    if (ev != null && ev.ErrorCode == ReturnValue.TakePictureAutoFocusNG)
                    {
                        //_dialogService.ShowInfo("Не удалось сфокусироваться. Пожалуйста, повторите попытку.");
                        Dispose();
                        Initialize();
                    }
                    if (ev != null && ev.ErrorCode == ReturnValue.NotSupported)
                        return;
                    break;
            }
        }
        private void ImageProcessorOnStreamChanged(object sender, ImageDto image)
        {
            Width = image.Width;
            Height = image.Height;
            if (image.ImageData.Length > 0)
            {
                LiveViewImageStream = image.ImageData;
                _cameraStreamSynchronize.Set();
            }
        }

        private void ImageProcessorCameraRemoveEvent(object sender, EventArgs e)
        {
            //Пытаемся открыть сессию если камера норм то показываем превью
            //_imageProcessor.CameraErrorEvent += ImageProcessorOnCameraErrorEvent;
            //_imageProcessor.ImageChanged += ImageProcessorOnStreamChanged;
            //_imageProcessor.CameraAddEvent += ImageProcessorCameraAddEvent;
            //_imageProcessor.CameraRemoveEvent += ImageProcessorCameraRemoveEvent;
            //_imageProcessor.InitializeProcessor();
            //CloseSession();//TODO Разобраться почему при включении камеры не приходит событие
            Dispose();
            _imageProcessor.SubscribeToCameraAddEvent();
            // Initialize();
            //    _imageProcessor.Dispose();
            PreviewReady = false;
        }

        private void ImageProcessorCameraAddEvent(object sender, EventArgs e)
        {
            //Пытаемся открыть сессию если камера норм то показываем превью
            _imageProcessor.InitializeProcessor();
            OpenSession();
            if (!_sessionOpened)
                return;

            StartLiveView();
            // Initialize();
        }
        private bool TakePhotoCanExecute()
        {
            return _takePhotoEnable;
        }


        public override void Dispose()
        {
            _imageProcessor.CameraErrorEvent -= ImageProcessorOnCameraErrorEvent;
            _imageProcessor.ImageChanged -= ImageProcessorOnStreamChanged;
            _imageProcessor.CameraAddEvent -= ImageProcessorCameraAddEvent;
            _imageProcessor.CameraRemoveEvent -= ImageProcessorCameraRemoveEvent;
            _sessionOpened = false;
            PreviewReady = false;
            _imageProcessor.Dispose();
        }
        #endregion

        #endregion

        #region Commands

        public RelayCommand SaveSettings => _saveSettings ?? (_saveSettings = new RelayCommand(Save, () => CameraSettings.CanSave));

        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public RelayCommand TakePhotoCommand
            => _takePhotoCommand ?? (_takePhotoCommand = new RelayCommand(async () => await TakePhoto(), TakePhotoCanExecute));

        public RelayCommand UndoCommand
        {
            get { return _undoCommand ?? (_undoCommand = new RelayCommand(() => CameraSettings.UndoChanges(), () => CameraSettings.CanUndo)); }
        }

        public RelayCommand RedoCommand
        {
            get { return _redoCommand ?? (_redoCommand = new RelayCommand(() => CameraSettings.RedoChanges(), () => CameraSettings.CanRedoChanges)); }
        }


        #endregion
    }
}
