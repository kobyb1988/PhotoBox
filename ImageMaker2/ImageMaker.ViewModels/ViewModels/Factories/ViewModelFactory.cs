using AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.CommonViewModels.ViewModels.Settings;
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
        private readonly IChildrenViewModelsFactory _welcomeViewModelChildFactory;

        public WelcomeViewModelFactory(
            IViewModelNavigator navigator, 
            IChildrenViewModelsFactory welcomeViewModelChildFactory)
        {
            _navigator = navigator;
            _welcomeViewModelChildFactory = welcomeViewModelChildFactory;
        }

        protected override WelcomeViewModel GetViewModel(object param)
        {
            return new WelcomeViewModel(_navigator, _welcomeViewModelChildFactory);
        }
    }

    public class SelectPatternViewModelFactory : ViewModelBaseFactory<SelectPatternViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IChildrenViewModelsFactory _childrenViewModelsFactory;
        private readonly PatternViewModelProvider _patternViewModelProvider;

        public SelectPatternViewModelFactory(
            IViewModelNavigator navigator, 
            IChildrenViewModelsFactory childrenViewModelsFactory, 
            PatternViewModelProvider patternViewModelProvider)
        {
            _navigator = navigator;
            _childrenViewModelsFactory = childrenViewModelsFactory;
            _patternViewModelProvider = patternViewModelProvider;
        }

        protected override SelectPatternViewModel GetViewModel(object param)
        {
            return new SelectPatternViewModel(_navigator, _childrenViewModelsFactory, _patternViewModelProvider);
        }
    }

    public class CameraViewModelFactory : ViewModelBaseFactory<CameraViewModel>
    {
        private readonly SettingsProvider _settings;
        private readonly IDialogService _dialogService;
        private readonly IChildrenViewModelsFactory _childrenViewModelsFactory;
        private readonly IViewModelNavigator _navigator;
        private readonly IMappingEngine _mappingEngine;
        private readonly CompositionModelProcessorFactory _imageProcessorFactory;

        public CameraViewModelFactory(
            SettingsProvider settings,
            IDialogService dialogService,
            IChildrenViewModelsFactory childrenViewModelsFactory, 
            IViewModelNavigator navigator, 
            IMappingEngine mappingEngine,
            CompositionModelProcessorFactory imageProcessorFactory
            )
        {
            _settings = settings;
            _dialogService = dialogService;
            _childrenViewModelsFactory = childrenViewModelsFactory;
            _navigator = navigator;
            _mappingEngine = mappingEngine;
            _imageProcessorFactory = imageProcessorFactory;
        }

        protected override CameraViewModel GetViewModel(object param)
        {
            //PatternData data = new PatternData() { Name = string.Empty, Id = 0, PatternType = PatternType.Simple, Data = new byte[] {0}};
            Composition composition = _mappingEngine.Map<Composition>(param);
            CompositionModelProcessor processor = _imageProcessorFactory.Create(composition);
            return new CameraViewModel(_settings, _dialogService, _childrenViewModelsFactory, _navigator, processor);
        }
    }

    public class CameraResultViewModelFactory : ViewModelBaseFactory<CameraResultViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly ImagePrinter _printer;
        private readonly SettingsProvider _settingsProvider;

        public CameraResultViewModelFactory(IViewModelNavigator navigator, ImagePrinter printer, SettingsProvider settingsProvider)
        {
            _navigator = navigator;
            _printer = printer;
            _settingsProvider = settingsProvider;
        }

        protected override CameraResultViewModel GetViewModel(object param)
        {
            return new CameraResultViewModel(_navigator, _printer, _settingsProvider, (CompositionProcessingResult)param);
        }
    }

    public class SelectActivityViewModelFactory : ViewModelBaseFactory<SelectActivityViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IChildrenViewModelsFactory _childrenViewModelsFactory;

        public SelectActivityViewModelFactory(
            SettingsProvider settingsProvider, 
            IViewModelNavigator navigator, 
            IChildrenViewModelsFactory childrenViewModelsFactory)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _childrenViewModelsFactory = childrenViewModelsFactory;
        }

        protected override SelectActivityViewModel GetViewModel(object param)
        {
            return new SelectActivityViewModel(_settingsProvider, _navigator, _childrenViewModelsFactory);
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

        public InstagramExplorerViewModelFactory(IViewModelNavigator navigator, InstagramExplorer instagramExplorer)
        {
            _navigator = navigator;
            _instagramExplorer = instagramExplorer;
        }

        protected override InstagramExplorerViewModel GetViewModel(object param)
        {
            return new InstagramExplorerViewModel(_navigator, _instagramExplorer);
        }
    }

    public class PrinterActivityViewerViewModelFactory : ViewModelBaseFactory<PrinterActivityViewerViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly PrinterMessageProvider _printerMessageProvider;
        private readonly SettingsProvider _settingsProvider;
        private readonly ImagePrinter _imagePrinter;

        public PrinterActivityViewerViewModelFactory(
            IViewModelNavigator navigator,
            PrinterMessageProvider printerMessageProvider,
            SettingsProvider settingsProvider,
            ImagePrinter imagePrinter)
        {
            _navigator = navigator;
            _printerMessageProvider = printerMessageProvider;
            _settingsProvider = settingsProvider;
            _imagePrinter = imagePrinter;
        }

        protected override PrinterActivityViewerViewModel GetViewModel(object param)
        {
            return new PrinterActivityViewerViewModel(_navigator, _printerMessageProvider, _imagePrinter, _settingsProvider);
        }
    }
}
