using System;
using GalaSoft.MvvmLight.Command;
using ImageMaker.CommonViewModels.Providers;
using ImageMaker.CommonViewModels.Services;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Navigation;

namespace ImageMaker.AdminViewModels.ViewModels.Passwords
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private PasswordBoxViewModel _passwordOld;
        private PasswordBoxViewModel _passwordNew;
        private PasswordBoxViewModel _passwordConfirm;
        private IViewModelNavigator _navigator;
        private RelayCommand _submitCommand;
        private RelayCommand _goBackCommand;
        SettingsProvider _settingsProvider;
        IDialogService _dialogService;

        public ChangePasswordViewModel(IViewModelNavigator navigator, SettingsProvider settingsProvider, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _navigator = navigator;
            _settingsProvider = settingsProvider;
            PasswordOld = new PasswordBoxViewModel();
            PasswordNew = new PasswordBoxViewModel();
            PasswordConfirm = new PasswordBoxViewModel();
            //todo Если есть идеи по лучше буду рад услышать, но пока так
            PasswordConfirm.OtherKeyboard = new[] { PasswordNew, PasswordOld };
            PasswordOld.OtherKeyboard = new[] { PasswordNew, PasswordConfirm };
            PasswordNew.OtherKeyboard = new[] { PasswordOld, PasswordConfirm };
        }

        public RelayCommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new RelayCommand(Submit));
            }
        }
        public RelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new RelayCommand(GoBack));
            }
        }

        private void GoBack()
        {
            _navigator.NavigateBack(this);
        }

        private void Submit()
        {
            PasswordOld.Error = PasswordNew.Error = PasswordConfirm.Error = "";
            if (string.IsNullOrEmpty(PasswordOld.Password))
            {
                PasswordOld.Error = "Пароль не введен";
                return;
            }
            
            if (string.IsNullOrEmpty(PasswordNew.Password))
            {
                PasswordNew.Error = "Новый пароль не введен";
                return;
            }
            
            if (string.IsNullOrEmpty(PasswordConfirm.Password))
            {
                PasswordConfirm.Error = "Подтверждение не введено";
                return;
            }
            
            if(PasswordConfirm.Password != PasswordNew.Password)
            {
                PasswordConfirm.Error = "Пароли не совпадают";
                return;
            }
            
            if (_settingsProvider.ChangePassword(PasswordOld.Password, PasswordNew.Password))
            {
                PasswordOld.Password = PasswordNew.Password = PasswordConfirm.Password = "";
                _dialogService.ShowInfo(@"Пароль изменен");
                return;
            }
            else
            {
                PasswordOld.Password = "";
                PasswordOld.Error = "Не верный пароль";
            }
        }

        public PasswordBoxViewModel PasswordOld
        {
            get { return _passwordOld; }
            set
            {
                if (_passwordOld == value)
                    return;
                _passwordOld = value;
                RaisePropertyChanged();
            }
        }

        public PasswordBoxViewModel PasswordNew
        {
            get { return _passwordNew; }
            set
            {
                if (_passwordNew == value)
                    return;

                _passwordNew = value;
                RaisePropertyChanged();
            }
        }

        public PasswordBoxViewModel PasswordConfirm
        {
            get { return _passwordConfirm; }
            set
            {
                if (_passwordConfirm == value)
                    return;
                _passwordConfirm = value;
                if (PasswordNew.Password != PasswordConfirm.Password)
                    throw new Exception("Пароли не совподают");
                RaisePropertyChanged();
            }
        }
    }
}
