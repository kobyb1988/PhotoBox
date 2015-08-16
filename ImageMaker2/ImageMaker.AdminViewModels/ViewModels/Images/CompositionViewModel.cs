using System.Collections.Generic;
using System.Linq;
using ImageMaker.CommonViewModels;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.AdminViewModels.ViewModels.Images
{
    public class CompositionViewModel : BaseViewModel
    {
        protected ImageViewModel _background;
        protected ImageViewModel _overlay;
        private string _name;

        public CompositionViewModel(
            string name,
            int id,
            TemplateViewModel template, 
            ImageViewModel background, 
            ImageViewModel overlay)
        {
            Name = name;
            Id = id;
            Template = template;
            Background = background;
            Overlay = overlay;
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

        public int Id { get; protected set; }
        public TemplateViewModel Template { get; protected set; }

        public List<TemplateImageData> TemplateImages
        {
            get
            {
                return Template.Children.Select(x => new TemplateImageData()
                {
                    X = x.X, 
                    Y = x.Y, 
                    Height = x.Height, 
                    Width = x.Width
                }).ToList();
            }
        } 

        public ImageViewModel Background
        {
            get { return _background; }
            set
            {
                _background = value;
                RaisePropertyChanged();
            }
        }

        public ImageViewModel Overlay
        {
            get { return _overlay; }
            set
            {
                _overlay = value;
                RaisePropertyChanged();
            }
        }
    }
}
