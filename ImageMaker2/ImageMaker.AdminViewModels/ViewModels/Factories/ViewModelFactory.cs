using AutoMapper;
using ImageMaker.AdminViewModels.Providers;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.Utils.Services;

namespace ImageMaker.AdminViewModels.ViewModels.Factories
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

    public class TemplateEditorViewModelFactory : ViewModelBaseFactory<TemplateEditorViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;

        public TemplateEditorViewModelFactory(
            IViewModelNavigator navigator,
            IDialogService dialogService)
        {
            _navigator = navigator;
            _dialogService = dialogService;
        }

        protected override TemplateEditorViewModel GetViewModel(object param)
        {
            return new TemplateEditorViewModel(_navigator, _dialogService, (CheckableTemplateViewModel) param);
        }
    }

    public class CompositionsEditorViewModelFactory : ViewModelBaseFactory<CompositionsEditorViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;

        public CompositionsEditorViewModelFactory(
            IViewModelNavigator navigator,
            IDialogService dialogService)
        {
            _navigator = navigator;
            _dialogService = dialogService;
        }

        protected override CompositionsEditorViewModel GetViewModel(object param)
        {
            return new CompositionsEditorViewModel(_navigator, _dialogService, (CheckableCompositionViewModel)param);
        }
    }

    public class TemplateExplorerViewModelFactory : ViewModelBaseFactory<TemplateExplorerViewModel>
    {
        private readonly IDialogService _dialogService;
        private readonly TemplateViewModelProvider _viewModelProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IChildrenViewModelsFactory _childrenViewModelsFactory;

        public TemplateExplorerViewModelFactory(
            IDialogService dialogService,
            TemplateViewModelProvider viewModelProvider,
            IViewModelNavigator navigator,
            IChildrenViewModelsFactory childrenViewModelsFactory)
        {
            _dialogService = dialogService;
            _viewModelProvider = viewModelProvider;
            _navigator = navigator;
            _childrenViewModelsFactory = childrenViewModelsFactory;
        }

        protected override TemplateExplorerViewModel GetViewModel(object param)
        {
            return new TemplateExplorerViewModel(_dialogService, _viewModelProvider, _navigator, _childrenViewModelsFactory);
        }
    }
    
    public class CompositionsExplorerViewModelFactory : ViewModelBaseFactory<CompositionsExplorerViewModel>
    {
        private readonly IDialogService _dialogService;
        private readonly TemplateProviderFactory _viewModelProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IChildrenViewModelsFactory _childrenViewModelsFactory;

        public CompositionsExplorerViewModelFactory(
            IDialogService dialogService,
            TemplateProviderFactory viewModelProvider,
            IViewModelNavigator navigator,
            IChildrenViewModelsFactory childrenViewModelsFactory)
        {
            _dialogService = dialogService;
            _viewModelProvider = viewModelProvider;
            _navigator = navigator;
            _childrenViewModelsFactory = childrenViewModelsFactory;
        }

        protected override CompositionsExplorerViewModel GetViewModel(object param)
        {
            return new CompositionsExplorerViewModel(_dialogService, _navigator, _childrenViewModelsFactory, _viewModelProvider);
        }
    }

    public class AppSettingsExplorerViewModelFactory : ViewModelBaseFactory<AppSettingsExplorerViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IMappingEngine _mappingEngine;
        private readonly SchedulerService _schedulerService;
        private readonly ImagePrinter _imagePrinter;

        public AppSettingsExplorerViewModelFactory(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator,
            IMappingEngine mappingEngine,
            SchedulerService schedulerService,
            ImagePrinter imagePrinter)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _mappingEngine = mappingEngine;
            _schedulerService = schedulerService;
            _imagePrinter = imagePrinter;
        }

        protected override AppSettingsExplorerViewModel GetViewModel(object param)
        {
            return new AppSettingsExplorerViewModel(_navigator, _settingsProvider, _mappingEngine, _schedulerService, _imagePrinter);
        }
    }

    public class CameraSettingsExplorerViewModelFactory : ViewModelBaseFactory<CameraSettingsExplorerViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IMappingEngine _mappingEngine;

        public CameraSettingsExplorerViewModelFactory(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator,
            IMappingEngine mappingEngine)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _mappingEngine = mappingEngine;
        }

        protected override CameraSettingsExplorerViewModel GetViewModel(object param)
        {
            return new CameraSettingsExplorerViewModel(_navigator, _settingsProvider, _mappingEngine);
        }
    }
}
