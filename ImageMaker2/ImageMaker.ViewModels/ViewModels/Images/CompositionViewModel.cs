using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.CommonViewModels;
using ImageMaker.CommonViewModels.ViewModels;

namespace ImageMaker.ViewModels.ViewModels.Images
{
    public class CompositionViewModel : BaseViewModel
    {
        public CompositionViewModel(
            int id, 
            int templateId,
            string name, 
            IEnumerable<TemplateImageData> templateImages,
            CommonViewModels.ViewModels.Images.ImageViewModel background, 
            CommonViewModels.ViewModels.Images.ImageViewModel overlay
            )
        {
            Id = id;
            Name = name;
            TemplateId = templateId;
            Background = background;
            Overlay = overlay;
            TemplateImages = new List<TemplateImageData>(templateImages);
        }

        public CommonViewModels.ViewModels.Images.ImageViewModel Overlay { get; private set; }
        public CommonViewModels.ViewModels.Images.ImageViewModel Background { get; private set; }

        public int Id { get; private set; }

        public string Name { get; set; }

        //public byte[] Background { get; set; }

        //public byte[] Overlay { get; set; }

        public int TemplateId { get; private set; }

        public List<TemplateImageData> TemplateImages { get; set; } 
    }
}
