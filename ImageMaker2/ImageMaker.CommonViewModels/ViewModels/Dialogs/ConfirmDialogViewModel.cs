using GalaSoft.MvvmLight.CommandWpf;

namespace ImageMaker.CommonViewModels.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel : DialogBase
    {
        public ConfirmDialogViewModel(string text)
        {
            Text = text;
        }

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

        public override string Title
        {
            get { return "Предупреждение"; }
        }
    }
}
