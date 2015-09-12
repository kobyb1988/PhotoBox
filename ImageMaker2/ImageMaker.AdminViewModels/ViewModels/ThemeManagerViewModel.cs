using System;
using System.Windows.Media;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Dto;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class ThemeManagerViewModel : BaseViewModel
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private readonly ImageLoadService _imageLoadService;

        private Color _mainWindowForegroundColor;
        private Color _mainWindowBackgroundColor;
        private Color _mainWindowBorderColor;
        private Color _otherWindowsBackgroundColor;
        private Color _otherWindowsForegroundColor;
        private Color _otherWindowsBorderColor;

        private ImageViewModel _mainWindowImage;
        private RelayCommand _goBackCommand;
        private RelayCommand<ColorType> _pickColorCommand;
        private RelayCommand _selectImageCommand;
        private RelayCommand _saveSettingsCommand;

        public ThemeManagerViewModel(
            IViewModelNavigator navigator, 
            IDialogService dialogService,
            SettingsProvider settingsProvider,
            IMappingEngine mappingEngine,
            ImageLoadService imageLoadService)
        {
            _navigator = navigator;
            _dialogService = dialogService;
            _settingsProvider = settingsProvider;
            _mappingEngine = mappingEngine;
            _imageLoadService = imageLoadService;
        }

        public override void Initialize()
        {
            ThemeSettingsDto settings = _settingsProvider.GetThemeSettings();
            if (settings == null)
            {
                MainWindowBackgroundColor = Colors.Orange;
                MainWindowBorderColor = Colors.Orange;
                MainWindowForegroundColor = Colors.White;

                OtherWindowsBackgroundColor = Colors.Orange;
                OtherWindowsBorderColor = Colors.Orange;
                OtherWindowsForegroundColor = Colors.White;
                return;
            }

            MainWindowImage = new ImageViewModel(settings.BackgroundImage);

            MainWindowBackgroundColor = settings.MainBackgroundColor;
            MainWindowBorderColor = settings.MainBorderColor;
            MainWindowForegroundColor = settings.MainForegroundColor;

            OtherWindowsBackgroundColor = settings.OtherBackgroundColor;
            OtherWindowsBorderColor = settings.OtherBorderColor;
            OtherWindowsForegroundColor = settings.OtherForegroundColor;
        }

        public RelayCommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack)); }
        }

        public RelayCommand SelectImageCommand
        {
            get { return _selectImageCommand ?? (_selectImageCommand = new RelayCommand(SelectImage)); }
        }

        public RelayCommand SaveSettingsCommand
        {
            get { return _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(SaveSettings)); }
        }

        private void SaveSettings()
        {
            _settingsProvider.SaveThemeSettings(_mappingEngine.Map<ThemeSettingsDto>(this));
        }

        private void SelectImage()
        {
            ImageViewModel viewModel = _imageLoadService.TryLoadImage();
            MainWindowImage = viewModel;
        }

        public RelayCommand<ColorType> PickColorCommand
        {
            get { return _pickColorCommand ?? (_pickColorCommand = new RelayCommand<ColorType>(PickColor)); }
        }

        private void PickColor(ColorType colorType)
        {
            Func<Color> getColor = null;
            Action<Color> setColor = null;

            switch (colorType)
            {
                case ColorType.MainBackground:
                    getColor = () => MainWindowBackgroundColor;
                    setColor = (color) => MainWindowBackgroundColor = color;
                    break;
                case ColorType.MainBorder:
                    getColor = () => MainWindowBorderColor;
                    setColor = (color) => MainWindowBorderColor = color;
                    break;
                case ColorType.MainForeground:
                    getColor = () => MainWindowForegroundColor;
                    setColor = (color) => MainWindowForegroundColor = color;
                    break;
                case ColorType.OtherBackground:
                    getColor = () => OtherWindowsBackgroundColor;
                    setColor = (color) => OtherWindowsBackgroundColor = color;
                    break;
                case ColorType.OtherBorder:
                    getColor = () => OtherWindowsBorderColor;
                    setColor = (color) => OtherWindowsBorderColor = color;
                    break;
                case ColorType.OtherForeground:
                    getColor = () => OtherWindowsForegroundColor;
                    setColor = (color) => OtherWindowsForegroundColor = color;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("colorType");
            }

            ColorPickerViewModel viewModel = new ColorPickerViewModel(getColor());
            bool result = _dialogService.ShowResultDialog(viewModel);
            if (!result)
                return;

            setColor(viewModel.SelectedColor);
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        public ImageViewModel MainWindowImage
        {
            get { return _mainWindowImage; }
            set
            {
                if (_mainWindowImage == value)
                    return;

                _mainWindowImage = value;
                RaisePropertyChanged();
            }
        }

        public Color MainWindowForegroundColor
        {
            get { return _mainWindowForegroundColor; }
            set
            {
                if (_mainWindowForegroundColor == value)
                    return;

                _mainWindowForegroundColor = value;
                RaisePropertyChanged();
            }
        }

        public Color MainWindowBackgroundColor
        {
            get { return _mainWindowBackgroundColor; }
            set
            {
                if (_mainWindowBackgroundColor == value)
                    return;

                _mainWindowBackgroundColor = value;
                RaisePropertyChanged();
            }
        }

        public Color MainWindowBorderColor
        {
            get { return _mainWindowBorderColor; }
            set
            {
                if (_mainWindowBorderColor == value)
                    return;

                _mainWindowBorderColor = value;
                RaisePropertyChanged();
            }
        }

        public Color OtherWindowsBackgroundColor
        {
            get { return _otherWindowsBackgroundColor; }
            set
            {
                if (_otherWindowsBackgroundColor == value)
                    return;

                _otherWindowsBackgroundColor = value;
                RaisePropertyChanged();
            }
        }

        public Color OtherWindowsForegroundColor
        {
            get { return _otherWindowsForegroundColor; }
            set
            {
                if (_otherWindowsForegroundColor == value)
                    return;

                _otherWindowsForegroundColor = value;
                RaisePropertyChanged();
            }
        }

        public Color OtherWindowsBorderColor
        {
            get { return _otherWindowsBorderColor; }
            set
            {
                if (_otherWindowsBorderColor == value)
                    return;

                _otherWindowsBorderColor = value;
                RaisePropertyChanged();
            }
        }
    }

    public enum ColorType
    {
        MainBackground,
        MainBorder,
        MainForeground,
        OtherBackground,
        OtherBorder,
        OtherForeground
    }
}
