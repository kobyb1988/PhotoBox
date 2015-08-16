using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.AdminViewModels.ViewModels.Enums;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class CheckableCompositionViewModel : CompositionViewModel, ICopyable<CheckableCompositionViewModel>
    {
        private bool _isChecked;
        private ItemState _state;

        public CheckableCompositionViewModel(CompositionViewModel viewModel)
            : base(viewModel.Name, viewModel.Id, viewModel.Template, viewModel.Background, viewModel.Overlay)
        {
        }

        public CheckableCompositionViewModel(CompositionViewModel viewModel, ItemState state)
            : this(viewModel)
        {
            State = state;
        }

        public CheckableCompositionViewModel(
            string name,
            int id, 
            TemplateViewModel template, 
            ImageViewModel background, 
            ImageViewModel overlay)
            : base(name, id, template, background, overlay)
        {
            State = ItemState.Added;
        }

        public static CheckableCompositionViewModel CreateEmpty(string name, TemplateViewModel template)
        {
            return new CheckableCompositionViewModel(name, 0, template, null, null);
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }

        public ItemState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged();
            }
        }

        public CheckableCompositionViewModel Copy()
        {
            return new CheckableCompositionViewModel(Name, Id, Template, Background, Overlay) { State = State };
        }

        public void CopyTo(CheckableCompositionViewModel to)
        {
            to.Id = Id;
            to.State = State;
            to.Background = Background;
            to.Overlay = Overlay;
            to.Template = Template;
            to.Name = Name;
            to.State = State;
        }
    }
}
