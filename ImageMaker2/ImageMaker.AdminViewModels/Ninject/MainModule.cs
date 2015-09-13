using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImageMaker.AdminViewModels.AutoMapper;
using ImageMaker.AdminViewModels.Providers;
using ImageMaker.AdminViewModels.ViewModels;
using ImageMaker.AdminViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.Data.Repositories;
using ImageMaker.Utils.Services;
using Ninject;
using Ninject.Modules;

namespace ImageMaker.AdminViewModels.Ninject
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMappingEngine>()
               .ToMethod(x => MappingEngineConfiguration.CreateEngine(new MainProfile()));

            Bind<ImagePrinter>().ToSelf();
            Bind<SettingsProvider>().ToSelf();
            Bind<TemplateViewModelProvider>().ToSelf();
            Bind<TemplateProviderFactory>().ToSelf();

            Bind<IImageRepository>().To<ImageRepository>();
            Bind<IUserRepository>().To<UserRepository>();

            Bind<SchedulerService>().ToSelf();

            Bind<IImageRepositoryFactory>().To<ImageRepositoryFactory>();
            Bind<ImageContextFactory>().ToSelf();

            Bind<MainViewModel>().ToSelf();
            Bind<WelcomeViewModel>().ToSelf();
            Bind<WelcomeViewModelFactory>().ToSelf();

            Bind<TemplateExplorerViewModel>().ToSelf();
            Bind<TemplateExplorerViewModelFactory>().ToSelf();

            Bind<StatsViewModelFactory>().ToSelf();
            Bind<TemplateEditorViewModel>().ToSelf();
            Bind<TemplateEditorViewModelFactory>().ToSelf();

            Bind<IChildrenViewModelsFactory>()
                .ToMethod(
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<InfoDialogViewModelFactory>(),
                                           x.Kernel.Get<ConfirmDialogViewModelFactory>(),
                                           x.Kernel.Get<ResultDialogViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    })
                .WhenInjectedExactlyInto<DialogService>();

            Bind<IViewModelNavigator>().To<ViewModelNavigator>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x => new ChildrenViewModelsFactory(Enumerable.Empty<IViewModelFactory>()));

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<MainViewModel>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                             x.Kernel.Get<WelcomeViewModelFactory>() //temporary
                                           //x.Kernel.Get<PasswordPromptViewModelFactory>()
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<PasswordPromptViewModelFactory>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<WelcomeViewModelFactory>()
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<WelcomeViewModelFactory>()
                .WithConstructorArgument(typeof (IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<TemplateExplorerViewModelFactory>(),
                                           x.Kernel.Get<AppSettingsExplorerViewModelFactory>(),
                                           x.Kernel.Get<CameraSettingsExplorerViewModelFactory>(),
                                           x.Kernel.Get<CompositionsExplorerViewModelFactory>(),
                                           x.Kernel.Get<ThemeManagerViewModelFactory>(),
                                           x.Kernel.Get<StatsViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<TemplateExplorerViewModelFactory>()
                .WithConstructorArgument(typeof (IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<TemplateEditorViewModelFactory>()
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<CompositionsExplorerViewModelFactory>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<CompositionsEditorViewModelFactory>()
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });
        }
    }
}
