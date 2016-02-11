﻿using GalaSoft.MvvmLight.Command;
using ImageMaker.CommonViewModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class PasswordBoxViewModel : BaseViewModel
    {
        public PasswordBoxViewModel()
        {
            IsShow = Visibility.Collapsed;
        }

        private string _password;

        public string Password
        {
            set
            {
                if (_password == value)
                    return;
                _password = value;
                RaisePropertyChanged();

            }
            get
            {
                return _password;
            }
        }
        private Visibility _isShow;
        public Visibility IsShow
        {
            get { return _isShow; }
            set
            {
                if (_isShow == value) return; _isShow = value; RaisePropertyChanged();
            }
        }
        private RelayCommand<string> _touchNumber;
        public RelayCommand<string> TouchNumber
        {
            get { return _touchNumber ?? (_touchNumber = new RelayCommand<string>(TouchNumberAction)); }
        }
        private RelayCommand _touchPasswordBox;
        public RelayCommand TouchPasswordBox { get { return _touchPasswordBox ?? (_touchPasswordBox = new RelayCommand(TouchPasswordBoxAction)); } }
        private RelayCommand _removeChar;
        public RelayCommand RemoveChar { get { return _removeChar ?? (_removeChar = new RelayCommand(RemoveCharAction)); } }

        private void TouchPasswordBoxAction()
        {
            IsShow = IsShow == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            if (IsShow == Visibility.Visible)
            {
                foreach (var other in OtherKeyboard)
                {
                    other.IsShow = Visibility.Collapsed;
                }
            }
        }
        private void RemoveCharAction()
        {
            if (Password.Length == 0)
                return;
            Password = Password.Substring(0, Password.Length - 1);
        }
        private void TouchNumberAction(string p)
        {
            Password += p;
        }
        public PasswordBoxViewModel[] OtherKeyboard { set; get; }

        public SolidColorBrush BorderBrush { set; get; }

        private string _error;
        public string Error
        {
            set
            {
                if (_error == value) return;
                _error = value;
                BorderBrush = string.IsNullOrEmpty(_error) ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.Red);
                RaisePropertyChanged(() => BorderBrush);
                RaisePropertyChanged();
            }
            get { return _error; }
        }
    }
}
