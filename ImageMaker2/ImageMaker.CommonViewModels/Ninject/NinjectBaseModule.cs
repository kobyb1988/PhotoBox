using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.AppServer;
using ImageMaker.Common.Helpers;
using ImageMaker.CommonViewModels.Messenger;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels.Navigation;
using Ninject.Modules;

namespace ImageMaker.CommonViewModels.Ninject
{
    public class NinjectBaseModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<IMappingEngine>()
            //    .ToMethod(x => MappingEngineConfigurator.CreateEngine(new BasicProfile()));

            Bind<ImageService>().ToSelf();
            Bind<ICommandProcessor>().To<CommandProcessor>();
            Bind<CommunicationManager>().ToSelf();
            Bind<MessageFactory>().ToSelf();
            Bind<IMessenger>().To<MvvmLightMessenger>().InSingletonScope();
            Bind<IHashBuilder>().To<HashBuilder>();
            Bind<ViewModelStorage>().ToSelf().InSingletonScope();
            Bind<IDialogService>().To<DialogService>();
        }
    }
}
