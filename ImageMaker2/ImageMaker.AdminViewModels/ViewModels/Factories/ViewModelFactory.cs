using AutoMapper;
using ImageMaker.AdminViewModels.Helpers;
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

    public class TemplateEditorViewModelFactory : ViewModelBaseFactory<TemplateEditorViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly ImageLoadService _imageLoadService;

        public TemplateEditorViewModelFactory(
            IViewModelNavigator navigator,
            IDialogService dialogService,
            ImageLoadService imageLoadService)
        {
            _navigator = navigator;
            _dialogService = dialogService;
            _imageLoadService = imageLoadService;
        }

        protected override TemplateEditorViewModel GetViewModel(object param)
        {
            return new TemplateEditorViewModel(_navigator, _dialogService, _imageLoadService, (CheckableTemplateViewModel) param);
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

        public TemplateExplorerViewModelFactory(
            IDialogService dialogService,
            TemplateViewModelProvider viewModelProvider,
            IViewModelNavigator navigator)
        {
            _dialogService = dialogService;
            _viewModelProvider = viewModelProvider;
            _navigator = navigator;
        }

        protected override TemplateExplorerViewModel GetViewModel(object param)
        {
            return new TemplateExplorerViewModel(_dialogService, _viewModelProvider, _navigator);
        }
    }
    
    public class CompositionsExplorerViewModelFactory : ViewModelBaseFactory<CompositionsExplorerViewModel>
    {
        private readonly IDialogService _dialogService;
        private readonly TemplateProviderFactory _viewModelProvider;
        private readonly IViewModelNavigator _navigator;

        public CompositionsExplorerViewModelFactory(
            IDialogService dialogService,
            TemplateProviderFactory viewModelProvider,
            IViewModelNavigator navigator
            )
        {
            _dialogService = dialogService;
            _viewModelProvider = viewModelProvider;
            _navigator = navigator;
        }

        protected override CompositionsExplorerViewModel GetViewModel(object param)
        {
            return new CompositionsExplorerViewModel(_dialogService, _navigator, _viewModelProvider);
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

    public class PasswordPromptViewModelFactory : ViewModelBaseFactory<PasswordPromptViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;

        public PasswordPromptViewModelFactory(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator,
            IDialogService dialogService
            )
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _dialogService = dialogService;
        }

        protected override PasswordPromptViewModel GetViewModel(object param)
        {
            return new PasswordPromptViewModel(_settingsProvider, _navigator, _dialogService);
        }
    }

    public class ThemeManagerViewModelFactory : ViewModelBaseFactory<ThemeManagerViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;
        private readonly ImageLoadService _imageLoadService;
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;

        public ThemeManagerViewModelFactory(
            IViewModelNavigator navigator,
            IDialogService dialogService,
            ImageLoadService imageLoadService,
            SettingsProvider settingsProvider,
            IMappingEngine mappingEngine
            )
        {
            _navigator = navigator;
            _dialogService = dialogService;
            _imageLoadService = imageLoadService;
            _settingsProvider = settingsProvider;
            _mappingEngine = mappingEngine;
        }

        protected override ThemeManagerViewModel GetViewModel(object param)
        {
            return new ThemeManagerViewModel(_navigator, _dialogService, _settingsProvider, _mappingEngine, _imageLoadService);
        }
    }

    public class StatsViewModelFactory : ViewModelBaseFactory<StatsViewModel>
    {
        private readonly IViewModelNavigator _navigator;

        public StatsViewModelFactory(
            IViewModelNavigator navigator
            )
        {
            _navigator = navigator;
        }

        protected override StatsViewModel GetViewModel(object param)
        {
            return new StatsViewModel(_navigator);
        }
    }
}
