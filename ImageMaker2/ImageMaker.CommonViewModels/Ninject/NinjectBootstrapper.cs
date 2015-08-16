using Ninject;
using Ninject.Modules;

namespace ImageMaker.CommonViewModels.Ninject
{
    public class NinjectBootstrapper
    {
        public static IKernel GetKernel(params INinjectModule[] mapping)
        {
            var kernel = new StandardKernel(mapping);
            return kernel;
        }
    }
}
