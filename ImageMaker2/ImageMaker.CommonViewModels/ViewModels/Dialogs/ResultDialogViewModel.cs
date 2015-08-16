using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace ImageMaker.CommonViewModels.ViewModels.Dialogs
{
    public class ResultDialogViewModel : DialogBase
    {
        private readonly string _title;

        public ResultDialogViewModel(ResultBaseViewModel content)
        {
            _title = content.Title;
            Content = content;
        }

        public ResultBaseViewModel Content { get; private set; }
        private RelayCommand<bool> _confirmCommand;

        public RelayCommand<bool> ConfirmCommand
        {
            get { return _confirmCommand ?? (_confirmCommand = new RelayCommand<bool>(Confirm)); }
        }

        private void Confirm(bool result)
        {
            Status = result;
            Close();
        }

        public string Text { get; private set; }

        public bool Status { get; private set; }

        public override string Title { get { return _title; } }
    }
}
