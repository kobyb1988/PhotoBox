using ImageMaker.CommonViewModels.ViewModels.Dialogs;

namespace ImageMaker.CommonViewModels.ViewModels.Factories
{
    public class ConfirmDialogViewModelFactory : ViewModelBaseFactory<ConfirmDialogViewModel>
    {
        protected override ConfirmDialogViewModel GetViewModel(object param)
        {
            return new ConfirmDialogViewModel(param.ToString());
        }
    }
}