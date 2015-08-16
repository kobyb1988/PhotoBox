using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels.ViewModels.Dialogs;

namespace ImageMaker.CommonViewModels.ViewModels.Factories
{
    public class ResultDialogViewModelFactory : ViewModelBaseFactory<ResultDialogViewModel>
    {
        protected override ResultDialogViewModel GetViewModel(object param)
        {
            return new ResultDialogViewModel((ResultBaseViewModel) param);
        }
    }
}
