using System;
using System.Runtime.InteropServices;
using AutoMapper;
using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.AdminViewModels.Providers;
using ImageMaker.AdminViewModels.Services;
using ImageMaker.AdminViewModels.ViewModels.CamerSettingsExplorer;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.AdminViewModels.ViewModels.Passwords;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.Utils.Services;
using NLog;

namespace ImageMaker.AdminViewModels.ViewModels.Factories
{
    public class WelcomeViewModelFactory : ViewModelBaseFactory<WelcomeViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settingsProvider;

        public WelcomeViewModelFactory(
            IViewModelNavigator navigator,
            SettingsProvider settingsProvider)
        {
            _navigator = navigator;
            _settingsProvider = settingsProvider;
        }

        protected override WelcomeViewModel GetViewModel(object param)
        {
            return new WelcomeViewModel(_navigator,_settingsProvider);
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

    public class ModuleManagedViewModelFactory : ViewModelBaseFactory<ModuleManagedViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IMappingEngine _mappingEngine;
        private readonly IViewModelNavigator _navigator;

        public ModuleManagedViewModelFactory(
            SettingsProvider settingsProvider,
            IMappingEngine mappingEngine,
            IViewModelNavigator navigator)
        {
            _settingsProvider = settingsProvider;
            _mappingEngine = mappingEngine;
            _navigator = navigator;
        }

        protected override ModuleManagedViewModel GetViewModel(object param)
        {
            return new ModuleManagedViewModel(_settingsProvider,_navigator,_mappingEngine);
        }
    }

    public class CameraSettingsExplorerViewModelFactory : ViewModelBaseFactory<CameraSettingsExplorerViewModel>
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly CompositionModelProcessor _imageProcessor;
        private readonly IMappingEngine _mappingEngine;
        private readonly IDialogService _dialogService;

        public CameraSettingsExplorerViewModelFactory(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator,
            CompositionModelProcessor imageProcessor,
            IMappingEngine mappingEngine,IDialogService dialogService)
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _imageProcessor = imageProcessor;
            _mappingEngine = mappingEngine;
            _dialogService = dialogService;
        }

        protected override CameraSettingsExplorerViewModel GetViewModel(object param)
        {
            return new CameraSettingsExplorerViewModel(_navigator, _settingsProvider, _imageProcessor,_mappingEngine, _dialogService);
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

    public class CurrentSessionViewModelFactory : ViewModelBaseFactory<CurrentSessionViewModel>
    {
        private readonly IViewModelNavigator _navigator;
        private readonly SessionService _sessionService;
        private readonly ImagePrinter _printer;
        private readonly SettingsProvider _settings;

        public CurrentSessionViewModelFactory(
            IViewModelNavigator navigator,
            SessionService sessionService,
            ImagePrinter printer,
            SettingsProvider settings
            )
        {
            _navigator = navigator;
            _sessionService = sessionService;
            _printer = printer;
            _settings = settings;
        }

        protected override CurrentSessionViewModel GetViewModel(object param)
        {
            return new CurrentSessionViewModel(_navigator, _sessionService, _printer, _settings);
        }
    }

    public class ChangePasswordViewModelFactory : ViewModelBaseFactory<ChangePasswordViewModel>
    {
        private readonly IDialogService _dialogService;
        private readonly IViewModelNavigator _navigator;
        private readonly SettingsProvider _settings;
        public ChangePasswordViewModelFactory(IViewModelNavigator navigator,SettingsProvider settings,IDialogService dialogService)
        {
            _dialogService = dialogService;
            _settings = settings;
            _navigator = navigator;
        }
        protected override ChangePasswordViewModel GetViewModel(object param)
        {
            return new ChangePasswordViewModel(_navigator, _settings, _dialogService);
        }
    }
}
