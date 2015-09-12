using System.Windows.Media;
using ImageMaker.CommonViewModels.ViewModels.Dialogs;
using ImageMaker.CommonViewModels.ViewModels.Validation;

namespace ImageMaker.AdminViewModels.ViewModels
{
    public class ColorPickerViewModel : ResultBaseViewModel
    {
        public ColorPickerViewModel(Color selectedColor)
        {
            SelectedColor = selectedColor;
        }

        private Color _selectedColor;

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor == value)
                    return;

                _selectedColor = value;
                RaisePropertyChanged();
            }
        }

        protected override ICustomValidationRule GetValidationRule()
        {
            return ValidationRule<ColorPickerViewModel>.Create(this);
        }

        public override bool CanConfirm
        {
            get { return true; }
        }

        public override string Title
        {
            get { return "Выберите цвет"; }
        }
    }
}
