using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Dialogs;
using ImageMaker.CommonViewModels.ViewModels.Validation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class ObjectNameViewModel : ResultBaseViewModel
    {
        private readonly string _title;

        public ObjectNameViewModel(string title)
        {
            _title = title;
        }

        private string _name;

        public override bool CanConfirm
        {
            get { return IsValid; }
        }

        public override string Title
        {
            get { return _title; }
        }

        protected override ICustomValidationRule GetValidationRule()
        {
            return ValidationRule<ObjectNameViewModel>.Create(this)
                .For(x => x.Name, x => !string.IsNullOrEmpty(x.Name), "Название не должно быть пустым", false);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }
    }
}
