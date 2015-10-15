namespace ImageMaker.CommonViewModels.ViewModels.Factories
{
    public abstract class ViewModelBaseFactory<TViewModel> : IViewModelFactory where TViewModel : BaseViewModel
    {
        public virtual BaseViewModel Get(object param)
        {
            return GetViewModel(param);
        }

        protected abstract TViewModel GetViewModel(object param);
    }
}