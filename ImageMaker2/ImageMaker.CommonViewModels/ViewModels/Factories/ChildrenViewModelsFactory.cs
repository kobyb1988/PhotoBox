using System.Collections.Generic;
using System.Linq;
using System.Monads;

namespace ImageMaker.CommonViewModels.ViewModels.Factories
{
    public interface IChildrenViewModelsFactory
    {
        BaseViewModel GetChild<TChild>(object param) where TChild : BaseViewModel;
    }

    public class ChildrenViewModelsFactory : IChildrenViewModelsFactory
    {
        private readonly IList<IViewModelFactory> _childFactories;

        public ChildrenViewModelsFactory(IEnumerable<IViewModelFactory> childFactories)
        {
            _childFactories = new List<IViewModelFactory>(childFactories);
        }

        public BaseViewModel GetChild<TChild>(object param) where TChild : BaseViewModel
        {
            return _childFactories.OfType<ViewModelBaseFactory<TChild>>().FirstOrDefault().Return(x => x.Get(param), null);
        }
    }
}