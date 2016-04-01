using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ImageMaker.Data.Repositories;
using Ninject;
using Ninject.Modules;

namespace ImageMaker.ScreenSaver.Ninject
{
    public class MainModule : NinjectModule
    {
        public override void Load()
        {
           
            Bind<CommonViewModels.Providers.SettingsProvider>().ToSelf();
            Bind<IImageRepository>().To<ImageRepository>();
            Bind<IUserRepository>().To<UserRepository>().InSingletonScope();
        }
    }
}
