using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Monads;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Extensions;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.MessageQueueing.MessageQueueing;
using ImageMaker.Utils.Services;
using ImageMaker.ViewModels.Providers;
using ImageMaker.ViewModels.ViewModels.Images;

namespace ImageMaker.ViewModels.ViewModels
{
    public class PrinterActivityViewerViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly PrinterMessageProvider _messageProvider;
        private readonly ImagePrinter _imagePrinter;
        private ObservableCollection<InstagramImageViewModel> _images;
        private RelayCommand _goBackCommand;
        private RelayCommand<IEnumerable<InstagramImageViewModel>> _selectImagesCommand;
        private int _copiesCount;
        private readonly string _printerName;

        private RelayCommand _printCommand;

        public PrinterActivityViewerViewModel(
            IViewModelNavigator navigator,
            PrinterMessageProvider messageProvider,
            ImagePrinter imagePrinter,
            SettingsProvider settingsProvider
            )
        {
            _navigator = navigator;
            _messageProvider = messageProvider;
            _imagePrinter = imagePrinter;
            _copiesCount = 1;
            AppSettingsDto appSettings = settingsProvider.GetAppSettings();
            if (appSettings != null)
            {
                HashTag = appSettings.HashTag;
                _printerName = appSettings.PrinterName;
            }
        }

        public override void Initialize()
        {
            _messageProvider.MessageDelivered += MessageProviderOnMessageDelivered;
            _messageProvider.StartListening();
        }

        public override void Dispose()
        {
            _messageProvider.MessageDelivered -= MessageProviderOnMessageDelivered;
            _messageProvider.InterruptListening();
        }

        public ObservableCollection<InstagramImageViewModel> Images
        {
            get { return _images ?? (_images = new ObservableCollection<InstagramImageViewModel>()); }
        }

        public RelayCommand<IEnumerable<InstagramImageViewModel>> SelectImagesCommand
        {
            get { return _selectImagesCommand ?? (_selectImagesCommand = new RelayCommand<IEnumerable<InstagramImageViewModel>>(SelectImages)); }
        }

        public InstagramImageViewModel SelectedImage
        {
            get { return _selectedImage; }
            set
            {
                if (_selectedImage == value)
                    return;

                _selectedImage = value;
                _canPrint = _selectedImage != null;
                PrintCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        private bool _canPrint;

        public RelayCommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new RelayCommand(Print, 
                () => _canPrint)); }
        }

        private void Print()
        {
            _imagePrinter.PrintAsync(SelectedImage.Data, _printerName, CopiesCount);
        }

        public int CopiesCount
        {
            get { return _copiesCount; }
            set
            {
                _copiesCount = value;
                RaisePropertyChanged();
            }
        }

        public string HashTag { get; private set; }

        private readonly Lazy<List<InstagramImageViewModel>> _selectedImages = new Lazy<List<InstagramImageViewModel>>();
        private InstagramImageViewModel _selectedImage;

        private void SelectImages(IEnumerable<InstagramImageViewModel> images)
        {
            images.Recover(Enumerable.Empty<InstagramImageViewModel>).CopyTo(_selectedImages.Value);
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        private void MessageProviderOnMessageDelivered(object sender, MessageEventArgs<InstagramImageViewModel> messageEventArgs)
        {
            UiInvoke(() => _images.Add(messageEventArgs.Content));
        }
    }
}
