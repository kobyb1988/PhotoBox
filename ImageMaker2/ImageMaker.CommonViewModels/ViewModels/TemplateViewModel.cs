using System.Collections.Generic;
using ImageMaker.CommonViewModels.ViewModels.Images;

namespace ImageMaker.CommonViewModels.ViewModels
{
    public class TemplateViewModel : BaseViewModel
    {
        public TemplateViewModel(int id, string name, IEnumerable<TemplateImageData> templateImages, ImageViewModel background, ImageViewModel overlay, bool isInstaPrinterTemplate)
        {
            Id = id;
            Name = name;
            Background = background;
            Overlay = overlay;
            TemplateImages = new List<TemplateImageData>(templateImages);
            IsInstaPrinterTemplate = isInstaPrinterTemplate;
        }

        public CommonViewModels.ViewModels.Images.ImageViewModel Overlay { get; private set; }
        public CommonViewModels.ViewModels.Images.ImageViewModel Background { get; private set; }

        public int Id { get; private set; }

        public string Name { get; set; }

        public bool IsInstaPrinterTemplate { get; set; }

        public List<TemplateImageData> TemplateImages { get; set; } 
    }
}
