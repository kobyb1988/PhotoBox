using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ImageMaker.CommonViewModels.ViewModels.Validation
{
    public abstract class ValidateableViewModel : BaseViewModel, IDataErrorInfo
    {
        private string _error;

        private readonly Lazy<ICustomValidationRule> _validationRule;

        protected ValidateableViewModel()
        {
            _validationRule = new Lazy<ICustomValidationRule>(GetValidationRule);
        }

        public string this[string columnName]
        {
            get { return _validationRule.Value.GetError(columnName); }
        }

        public bool IsValid
        {
            get { return _validationRule.Value.IsValidObject; }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged();
            }
        }

        protected abstract ICustomValidationRule GetValidationRule();
    }
}