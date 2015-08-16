using ImageMaker.AdminViewModels.ViewModels.Enums;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public static class Extensions
    {
        public static CheckableTemplateViewModel ToCheckable(this TemplateViewModel source)
        {
            return new CheckableTemplateViewModel(source);
        }

        public static CheckableTemplateViewModel ToCheckable(this TemplateViewModel source, ItemState state)
        {
            return new CheckableTemplateViewModel(source, state);
        }

        public static CheckableCompositionViewModel ToCheckable(this CompositionViewModel source)
        {
            return new CheckableCompositionViewModel(source);
        }

        public static CheckableCompositionViewModel ToCheckable(this CompositionViewModel source, ItemState state)
        {
            return new CheckableCompositionViewModel(source, state);
        }
    }
}