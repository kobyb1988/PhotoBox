namespace ImageMaker.CommonViewModels.ViewModels.Navigation
{
    public interface IViewModelNavigator
    {
        void NavigateBack(BaseViewModel viewModel);
        void NavigateForward(BaseViewModel from, BaseViewModel to);
        void NavigateForward(BaseViewModel to);

        void NavigateForward<TViewModelTo>(BaseViewModel from, object param) where TViewModelTo : BaseViewModel;
        void NavigateForward<TViewModelTo>(object param) where TViewModelTo : BaseViewModel;
    }
}