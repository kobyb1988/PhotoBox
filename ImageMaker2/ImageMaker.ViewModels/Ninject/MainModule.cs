using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImageMaker.CommonViewModels.AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Factories;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using ImageMaker.Data;
using ImageMaker.Data.Repositories;
using ImageMaker.MessageQueueing.MessageQueueing;
using ImageMaker.PatternProcessing.ImageProcessors;
using ImageMaker.Utils.Services;
using ImageMaker.ViewModels.AutoMapper;
using ImageMaker.ViewModels.Providers;
using ImageMaker.ViewModels.ViewModels;
using ImageMaker.ViewModels.ViewModels.Factories;
using ImageMaker.WebBrowsing;
using Ninject;
using Ninject.Modules;

namespace ImageMaker.ViewModels.Ninject
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMappingEngine>()
                .ToMethod(x => MappingEngineConfiguration.CreateEngine(new MainProfile()));
                //.WhenInjectedExactlyInto<PatternViewModelProvider>();

            Bind<IPatternDataProvider>().To<PatternDataProvider>();
            Bind<IImageDataProvider>().To<ImageDataProvider>();
            Bind<IImageRepository>().To<ImageRepository>();
            Bind<IUserRepository>().To<UserRepository>();

            Bind<SettingsProvider>().ToSelf();
            Bind<PatternViewModelProvider>().ToSelf();
            Bind<PrinterMessageProvider>().ToSelf();

            Bind<IPatternRepository>().To<PatternRepository>();
            Bind<PatternManageViewModelProvider>().ToSelf();

            Bind<QueueListenerFactory>().ToSelf();
            Bind<PrinterActivityViewerViewModelFactory>().ToSelf();
            
            Bind<ImagePrinter>().ToSelf();
            Bind<InstagramExplorer>().ToSelf();
            Bind<MainViewModel>().ToSelf();
            Bind<WelcomeViewModel>().ToSelf();
            Bind<CompositionModelProcessorFactory>().ToSelf();
            Bind<WelcomeViewModelFactory>().ToSelf();
            Bind<SelectPatternViewModelFactory>().ToSelf();
            Bind<CameraViewModelFactory>().ToSelf();
            Bind<SelectActivityViewModelFactory>().ToSelf();
            Bind<ImportPatternsViewModelFactory>().ToSelf();
            Bind<InstagramExplorerViewModelFactory>().ToSelf();

            Bind<IViewModelNavigator>().To<ViewModelNavigator>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x => new ChildrenViewModelsFactory(Enumerable.Empty<IViewModelFactory>()));

            Bind<IChildrenViewModelsFactory>()
                .ToMethod(
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<InfoDialogViewModelFactory>(),
                                           x.Kernel.Get<ConfirmDialogViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    })
                .WhenInjectedExactlyInto<DialogService>();

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<MainViewModel>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<WelcomeViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<WelcomeViewModelFactory>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<SelectActivityViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<SelectActivityViewModelFactory>()
                .WithConstructorArgument(typeof (IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<SelectPatternViewModelFactory>(),
                                           x.Kernel.Get<ImportPatternsViewModelFactory>(),
                                           x.Kernel.Get<InstagramExplorerViewModelFactory>(),
                                           x.Kernel.Get<PrinterActivityViewerViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<SelectPatternViewModelFactory>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<CameraViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });

            Bind<IViewModelNavigator>()
                .To<ViewModelNavigator>()
                .WhenInjectedExactlyInto<CameraViewModelFactory>()
                .WithConstructorArgument(typeof(IChildrenViewModelsFactory),
                    x =>
                    {
                        var children = new List<IViewModelFactory>
                                       {
                                           x.Kernel.Get<CameraResultViewModelFactory>(),
                                       };

                        return new ChildrenViewModelsFactory(children);
                    });
        }
    }
}
