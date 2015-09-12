using System.Collections.Generic;
using ImageMaker.AdminViewModels.Helpers;
using ImageMaker.AdminViewModels.ViewModels.Enums;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class CheckableTemplateViewModel : TemplateViewModel, ICopyable<CheckableTemplateViewModel>
    {
        private bool _isChecked;
        private ItemState _state;

        public CheckableTemplateViewModel(TemplateViewModel viewModel)
            : base(viewModel.Name, viewModel.Width, viewModel.Height, viewModel.Id, viewModel.Children, viewModel.Background, viewModel.Overlay)
        {
        }

        public CheckableTemplateViewModel(TemplateViewModel viewModel, ItemState state)
            : this(viewModel)
        {
            State = state;
        }

        public CheckableTemplateViewModel(
            string name, 
            uint width, 
            uint height, 
            int id, 
            IEnumerable<TemplateImageViewModel> children, 
            ImageViewModel background, 
            ImageViewModel overlay)
            : base(name, width, height, id, children, background, overlay)
        {
            State = ItemState.Added;
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

        public CheckableTemplateViewModel Copy()
        {
            return new TemplateViewModel(Name, Width, Height, Id, Children, Background, Overlay).ToCheckable(State);
        }

        public void CopyTo(CheckableTemplateViewModel to)
        {
            to._width = Width;
            to._height = Height;
            to.Id = Id;

            to.Background = Background;
            to.Overlay = Overlay;

            to.State = State;

            to.Children.Clear();

            foreach (var child in Children)
            {
                to.Children.Add(child);
            }
        }
    }
}