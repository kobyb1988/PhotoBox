namespace ImageMaker.CommonViewModels.ViewModels.Factories
{
    public interface IViewModelFactory
    {
        BaseViewModel Get(object param);
    }
}