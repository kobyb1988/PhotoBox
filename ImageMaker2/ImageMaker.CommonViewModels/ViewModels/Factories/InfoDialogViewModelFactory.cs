using ImageMaker.CommonViewModels.ViewModels.Dialogs;

namespace ImageMaker.CommonViewModels.ViewModels.Factories
{
    public class InfoDialogViewModelFactory : ViewModelBaseFactory<InfoDialogViewModel>
    {
        protected override InfoDialogViewModel GetViewModel(object param)
        {
            return new InfoDialogViewModel(param.ToString());
        }
    }
}