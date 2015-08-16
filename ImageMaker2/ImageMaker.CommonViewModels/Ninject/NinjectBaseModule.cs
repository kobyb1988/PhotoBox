using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Bind<MessageFactory>().ToSelf();
            Bind<IMessenger>().To<MvvmLightMessenger>().InSingletonScope();
            Bind<IViewModelNavigator>().To<ViewModelNavigator>().InSingletonScope();

            Bind<IDialogService>().To<DialogService>();
            

        }
    }
}
