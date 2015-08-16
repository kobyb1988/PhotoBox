using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using AutoMapper;
using ImageMaker.AdminViewModels.AutoMapper;
using ImageMaker.AdminViewModels.Providers;
using ImageMaker.AdminViewModels.ViewModels;
using ImageMaker.AdminViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.Data;
using ImageMaker.Data.Repositories;
using ImageMaker.Entities;
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

            Bind<TemplateEditorViewModel>().ToSelf();
            Bind<TemplateEditorViewModelFactory>().ToSelf();

            Bind<IChildrenViewModelsFactory>().ToMethod(x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<InfoDialogViewModelFactory>(),
                    x.Kernel.Get<ConfirmDialogViewModelFactory>(),
                    x.Kernel.Get<ResultDialogViewModelFactory>(),
                };

                return new ChildrenViewModelsFactory(children);
            }).WhenInjectedExactlyInto<DialogService>();

            Bind<IChildrenViewModelsFactory>().ToMethod(x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<WelcomeViewModelFactory>()
                };

                return new ChildrenViewModelsFactory(children);
            }).WhenInjectedExactlyInto<MainViewModel>();

            Bind<IChildrenViewModelsFactory>().ToMethod(x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<TemplateExplorerViewModelFactory>(),
                    x.Kernel.Get<AppSettingsExplorerViewModelFactory>(),
                    x.Kernel.Get<CameraSettingsExplorerViewModelFactory>(),
                    x.Kernel.Get<CompositionsExplorerViewModelFactory>(),
                };

                return new ChildrenViewModelsFactory(children);
            }).WhenInjectedExactlyInto<WelcomeViewModelFactory>();

            Bind<IChildrenViewModelsFactory>().ToMethod(x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<TemplateEditorViewModelFactory>()
                };

                return new ChildrenViewModelsFactory(children);
            }).WhenInjectedExactlyInto<TemplateExplorerViewModelFactory>();

            Bind<IChildrenViewModelsFactory>().ToMethod(x =>
            {
                var children = new List<IViewModelFactory>
                {
                    x.Kernel.Get<CompositionsEditorViewModelFactory>()
                };

                return new ChildrenViewModelsFactory(children);
            }).WhenInjectedExactlyInto<CompositionsExplorerViewModelFactory>();
        }
    }
}
