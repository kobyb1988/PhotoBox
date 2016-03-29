using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.Common.Helpers;
using ImageMaker.CommonViewModels.AutoMapper;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.Data.Repositories;
using ImageMaker.MessageQueueing.MessageQueueing;
using ImageMaker.Utils.Services;
using ImageMaker.WebBrowsing;
using InstagramImagePrinter.AutoMapper;
using Ninject.Modules;

namespace InstagramImagePrinter.Ninject
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IMappingEngine>()
               .ToMethod(x => MappingEngineConfiguration.CreateEngine(new MainProfile()));

            Bind<IHashBuilder>().To<HashBuilder>();
            Bind<IImageRepository>().To<ImageRepository>();
            Bind<ImageService>().ToSelf();
            Bind<MessageAdapter>().ToSelf();
            Bind<ImageContextFactory>().ToSelf();
            Bind<IUserRepository>().To<UserRepository>().InSingletonScope();
            Bind<SettingsProvider>().ToSelf();
            Bind<ImagePrinter>().ToSelf();
            Bind<InstagramExplorer>().ToSelf();
            Bind<QueueUtilizerFactory>().ToSelf();
            Bind<MonitoringService>().ToSelf();
        }
    }
}
