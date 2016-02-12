using System;
using System.Diagnostics;
using System.Windows.Media;
using AutoMapper;
using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.Common.Dto;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using System.Windows;
using System.Collections.Generic;

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
        private Color _otherWindowsButtonColor;
        private Color _otherWindowsForegroundButtonColor;
        private Color _otherWindowsBackgroundCircleColor;

        private bool _isBackToDefaultTheme;
        private ImageViewModel _mainWindowImage;
        private RelayCommand _backToDefaultThemeCommand;
        private RelayCommand _goBackCommand;
        private RelayCommand<ColorType> _pickColorCommand;
        private RelayCommand _selectImageCommand;
        private RelayCommand _saveSettingsCommand;
        private ImageViewModel _otherWindowsImage;
        private RelayCommand _selectOtherWindowsImageCommand;
        private ImageViewModel _backgroundImage;
        private RelayCommand _selectBackgroundImageCommand;
        private RelayCommand _previewCommand;
        private Visibility _showMainPreview;
        private Visibility _showOtherPreview;
        public Visibility ShowMainPreview
        {
            set
            {
                _showMainPreview = value;
                RaisePropertyChanged();
            }
            get
            {
                return _showMainPreview;
            }
        }
        public Visibility ShowOtherPreview
        {
            set
            {
                _showOtherPreview = value;
                RaisePropertyChanged();
            }
            get
            {
                return _showOtherPreview;
            }
        }

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
            ShowOtherPreview = Visibility.Collapsed;
            if (settings == null)
            {
                BackToDefaultTheme();
                return;
            }

            if (settings.BackgroundImage != null)
                MainWindowImage = new ImageViewModel(settings.BackgroundImage);

            if (settings.OtherBackgroundImage != null)
                OtherWindowsImage = new ImageViewModel(settings.OtherBackgroundImage);
            MainWindowBackgroundColor = settings.MainBackgroundColor;
            MainWindowBorderColor = settings.MainBorderColor;
            MainWindowForegroundColor = settings.MainForegroundColor;

            OtherWindowsBackgroundColor = settings.OtherBackgroundColor;
            OtherWindowsBorderColor = settings.OtherBorderColor;
            OtherWindowsForegroundColor = settings.OtherForegroundColor;
            OtherWindowsButtonColor = settings.OtherButtonColor;
            OtherWindowsForegroundButtonColor = settings.OtherForegroundButtonColor;
            OtherWindowsBackgroundCircleColor = settings.OtherBackgroundCircleColor;
            
        }

        public RelayCommand PreviewCommand => _previewCommand ?? (_previewCommand = new RelayCommand(GoPreview));
        public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));

        public RelayCommand SelectBackgroundImageCommand => _selectBackgroundImageCommand ?? (_selectBackgroundImageCommand = new RelayCommand(SelectBackgroundImage));

        private void SelectBackgroundImage()
        {
            ImageViewModel viewModel = _imageLoadService.TryLoadImage();
            BackgroundImage = viewModel;
        }

        public RelayCommand SelectImageCommand => _selectImageCommand ?? (_selectImageCommand = new RelayCommand(SelectImage));

        public RelayCommand SelectOtherWindowsImageCommand => _selectOtherWindowsImageCommand ?? (_selectOtherWindowsImageCommand = new RelayCommand(SelectOtherWindowsImage));

        private void GoPreview()
        {
            if (ShowMainPreview == Visibility.Visible)
            {
                ShowMainPreview = Visibility.Collapsed;
                ShowOtherPreview = Visibility.Visible;
            }
            else
            {
                ShowMainPreview = Visibility.Visible;
                ShowOtherPreview = Visibility.Collapsed;
            }
        }

        private void SelectOtherWindowsImage()
        {
            ImageViewModel viewModel = _imageLoadService.TryLoadImage();
            OtherWindowsImage = viewModel;
        }

        public RelayCommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(SaveSettings));

        private void SaveSettings()
        {
            if (IsBackToDefaultTheme)
            {
                BackToDefaultTheme();
                _settingsProvider.SaveThemeSettings(null);
            }
            else
                _settingsProvider.SaveThemeSettings(_mappingEngine.Map<ThemeSettingsDto>(this));

        }

        private void SelectImage()
        {
            ImageViewModel viewModel = _imageLoadService.TryLoadImage();
            MainWindowImage = viewModel;
        }

        public RelayCommand<ColorType> PickColorCommand => _pickColorCommand ?? (_pickColorCommand = new RelayCommand<ColorType>(PickColor));

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
                case ColorType.OtherButton:
                    getColor = () => OtherWindowsButtonColor;
                    setColor = c => OtherWindowsButtonColor = c;
                    break;
                case ColorType.OtherForegroundButton:
                    getColor = () => OtherWindowsForegroundButtonColor;
                    setColor = c => OtherWindowsForegroundButtonColor = c;
                    break;
                case ColorType.OtherBackgroundCircleButton:
                    getColor = () => OtherWindowsBackgroundCircleColor;
                    setColor = c => OtherWindowsBackgroundCircleColor = c;
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

        private void BackToDefaultTheme()
        {
            MainWindowBackgroundColor = Colors.Orange;
            MainWindowBorderColor = Colors.Orange;
            MainWindowForegroundColor = Colors.White;

            OtherWindowsBackgroundColor = Colors.Orange;
            OtherWindowsBorderColor = Colors.Orange;
            OtherWindowsForegroundColor = Colors.White;
            OtherWindowsButtonColor = Colors.Orange;
            OtherWindowsForegroundButtonColor = Colors.White;
            OtherWindowsBackgroundCircleColor = Colors.Orange;
        }

        public bool IsBackToDefaultTheme
        {
            get { return _isBackToDefaultTheme; }
            set
            {
                _isBackToDefaultTheme = value;
                RaisePropertyChanged();
            }
        }

        public ImageViewModel BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                if (_backgroundImage == value)
                    return;

                _backgroundImage = value;
                RaisePropertyChanged();
            }
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

        public ImageViewModel OtherWindowsImage
        {
            get { return _otherWindowsImage; }
            set
            {
                if (_otherWindowsImage == value)
                    return;

                _otherWindowsImage = value;
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

        public Color OtherWindowsButtonColor
        {
            get { return _otherWindowsButtonColor; }
            set
            {
                if (_otherWindowsButtonColor == value)
                    return;

                _otherWindowsButtonColor = value;
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

        public Color OtherWindowsForegroundButtonColor
        {
            get { return _otherWindowsForegroundButtonColor; }
            set
            {
                if (_otherWindowsForegroundButtonColor == value)
                    return;

                _otherWindowsForegroundButtonColor = value;
                RaisePropertyChanged();
            }
        }

        public Color OtherWindowsBackgroundCircleColor
        {
            get { return _otherWindowsBackgroundCircleColor; }
            set
            {
                if (_otherWindowsBackgroundCircleColor == value)
                    return;

                _otherWindowsBackgroundCircleColor = value;
                RaisePropertyChanged();
            }
        }

        public string TextCheck { get{ return "Проврека"; }}
    }

    public enum ColorType
    {
        MainBackground,
        MainBorder,
        MainForeground,
        OtherBackground,
        OtherBorder,
        OtherForeground,
        OtherButton,
        OtherForegroundButton,
        OtherBackgroundCircleButton
    }
}
