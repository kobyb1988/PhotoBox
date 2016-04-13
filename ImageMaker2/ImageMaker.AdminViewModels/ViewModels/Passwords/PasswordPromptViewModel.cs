using GalaSoft.MvvmLight.CommandWpf;
using ImageMaker.CommonViewModels.Behaviors;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels.Passwords
{
    public class PasswordPromptViewModel : BaseViewModel, IPassword
    {
        private readonly SettingsProvider _settingsProvider;
        private readonly IViewModelNavigator _navigator;
        private readonly IDialogService _dialogService;

        private RelayCommand _submitCommand;
        private RelayCommand<string> _touchCommand;
        private string _password;

        public PasswordPromptViewModel(
            SettingsProvider settingsProvider,
            IViewModelNavigator navigator,
            IDialogService dialogService
            )
        {
            _settingsProvider = settingsProvider;
            _navigator = navigator;
            _dialogService = dialogService;
        }

        public RelayCommand SubmitCommand
            => _submitCommand ?? (_submitCommand = new RelayCommand(Submit, IsPasswordTyped));

        public RelayCommand<string> TouchNumber
            => _touchCommand ?? (_touchCommand = new RelayCommand<string>(TouchNumberPw));

        private RelayCommand _removeChar;

        public RelayCommand RemoveChar
            => _removeChar ?? (_removeChar = new RelayCommand(RemoveCharAction, IsPasswordTyped));

        private void RemoveCharAction()
        {
            if (Password.Length == 0)
                return;
            Password = Password.Substring(0, Password.Length - 1);
        }

        private void TouchNumberPw(string p)
        {
            Password += p; 
        }

        private bool IsPasswordTyped()
        {
            return !string.IsNullOrEmpty(Password);
        }

        private void Submit()
        {
            var result = _settingsProvider.ValidatePassword(Password);
            if (!result)
            {
                _dialogService.ShowInfo(@"Неверный пароль");
                Password = string.Empty;
                return;
            }

            _navigator.NavigateForward<WelcomeViewModel>(null);
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value)
                    return;

                _password = value;
                RaisePropertyChanged();
            }
        }
    }
}
