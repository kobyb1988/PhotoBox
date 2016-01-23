using System;
using System.Diagnostics;
using AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.Entities;
using ImageMaker.PatternProcessing;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.Utils.Services;
using ImageMaker.ViewModels.Providers;
using ImageMaker.WebBrowsing;

namespace ImageMaker.ViewModels.ViewModels.Factories
{
    public class WelcomeViewModelFactory : ViewModelBaseFactory<WelcomeViewModel>
    {
        private readonly IViewModelNavigator _navigator;

        public WelcomeViewModelFactory(
            IViewModelNavigator navigator)
        {
            _navigator = navigator;
        }

        protected override WelcomeViewModel GetViewModel(object param)
        {
            return new WelcomeViewModel(_navigator);
        }
    }

    public class SelectPatternViewModelFactory : ViewModelBaseFactory<SelectPatternViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly PatternViewModelProvider _patternViewModelProvider;

        public SelectPatternViewModelFactory(
            IViewModelNavigator navigator, 
            PatternViewModelProvider patternViewModelProvider)
        {
            _navigator = navigator;
            _patternViewModelProvider = patternViewModelProvider;
        }

        protected override SelectPatternViewModel GetViewModel(object param)
        {
            return new SelectPatternViewModel(_navigator, _patternViewModelProvider);
        }
    }

    public class CameraViewModelFactory : ViewModelBaseFactory<CameraViewModel>
    {
        private readonly SettingsProvider _settings;
        private readonly IDialogService _dialogService;
        private readonly IViewModelNavigator _navigator;
        private readonly IMappingEngine _mappingEngine;
        private readonly CompositionModelProcessorFactory _imageProcessorFactory;

        public CameraViewModelFactory(
            SettingsProvider settings,
            IDialogService dialogService,
            IViewModelNavigator navigator, 
            IMappingEngine mappingEngine,
            CompositionModelProcessorFactory imageProcessorFactory
            )
        {
            _settings = settings;
            _dialogService = dialogService;
            _navigator = navigator;
            _mappingEngine = mappingEngine;
            _imageProcessorFactory = imageProcessorFactory;
        }

        protected override CameraViewModel GetViewModel(object param)
        {
            //PatternData data = new PatternData() { Name = string.Empty, Id = 0, PatternType = PatternType.Simple, Data = new byte[] {0}};
            Template composition = _mappingEngine.Map<Template>(param);
            CompositionModelProcessor processor = _imageProcessorFactory.Create(composition);
            return new CameraViewModel(_settings, _dialogService, _navigator, processor);
        }
    }

    public class CameraResultViewModelFactory : ViewModelBaseFactory<CameraResultViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly ImagePrinter _printer;
        private readonly SettingsProvider _settingsProvider;
        private readonly ImageService _imageService;

        public CameraResultViewModelFactory(
            IViewModelNavigator navigator,
            ImagePrinter printer, 
            SettingsProvider settingsProvider,
            ImageService imageService)
        {
            _navigator = navigator;
            _printer = printer;
            _settingsProvider = settingsProvider;
            _imageService = imageService;
        }

        protected override CameraResultViewModel GetViewModel(object param)
        {
            return new CameraResultViewModel(_navigator, _printer, _settingsProvider, _imageService, (CompositionProcessingResult)param);
        }
    }

    public class SelectActivityViewModelFactory : ViewModelBaseFactory<SelectActivityViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;

        public SelectActivityViewModelFactory(
            SettingsProvider settingsProvider, 
            IViewModelNavigator navigator)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
        }

        protected override SelectActivityViewModel GetViewModel(object param)
        {
            return new SelectActivityViewModel(_settingsProvider, _navigator);
        }
    }

    public class ImportPatternsViewModelFactory : ViewModelBaseFactory<ImportPatternsViewModel>
    {
        private readonly PatternManageViewModelProvider _viewModelProvider;
        private readonly IDialogService _dialogService;
        private readonly IViewModelNavigator _navigator;

        public ImportPatternsViewModelFactory(
            PatternManageViewModelProvider viewModelProvider,
            IDialogService dialogService,
            IViewModelNavigator navigator)
        {
            _viewModelProvider = viewModelProvider;
            _dialogService = dialogService;
            _navigator = navigator;
        }

        protected override ImportPatternsViewModel GetViewModel(object param)
        {
            return new ImportPatternsViewModel(_navigator, _dialogService, _viewModelProvider);
        }
    }

    public class InstagramExplorerViewModelFactory : ViewModelBaseFactory<InstagramExplorerViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly InstagramExplorer _instagramExplorer;
        private readonly SettingsProvider _settings;
        private readonly ImagePrinter _printer;
        private readonly PatternViewModelProvider _patternVmProvider;
        private readonly ImageUtils _imageUtils;
        private readonly IMappingEngine _mappingEngine;

        public InstagramExplorerViewModelFactory(
            IViewModelNavigator navigator, 
            InstagramExplorer instagramExplorer,
            SettingsProvider settings,
            ImagePrinter printer, PatternViewModelProvider patternVMProvider,
            ImageUtils imageUtils,IMappingEngine mappingEngine)
        {
            _navigator = navigator;
            _instagramExplorer = instagramExplorer;
            _settings = settings;
            _printer = printer;
            _patternVmProvider = patternVMProvider;
            _imageUtils = imageUtils;
            _mappingEngine = mappingEngine;
        }

        protected override InstagramExplorerViewModel GetViewModel(object param)
        {
            return new InstagramExplorerViewModel(_navigator, _instagramExplorer, _settings,
                _printer, _patternVmProvider,_imageUtils,_mappingEngine);
        }
    }

    public class PrinterActivityViewerViewModelFactory : ViewModelBaseFactory<PrinterActivityViewerViewModel>
    {
        private readonly Lazy<PrinterActivityViewerViewModel> _printerActivityViewModel; 

        public PrinterActivityViewerViewModelFactory(
            IViewModelNavigator navigator,
            PrinterMessageProvider printerMessageProvider,
            SettingsProvider settingsProvider,
            ImagePrinter imagePrinter)
        {
            _printerActivityViewModel = new Lazy<PrinterActivityViewerViewModel>(() =>
                new PrinterActivityViewerViewModel(navigator, printerMessageProvider, imagePrinter, settingsProvider));
        }

        protected override PrinterActivityViewerViewModel GetViewModel(object param)
        {
            return _printerActivityViewModel.Value;
        }
    }
}
