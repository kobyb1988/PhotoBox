using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using AutoMapper;
using ImageMaker.CommonViewModels;
using ImageMaker.CommonViewModels.ViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Entities;
using ImageMaker.MessageQueueing.Dto;
using Image = ImageMaker.WebBrowsing.Image;

namespace InstagramImagePrinter.AutoMapper
{
    public class MainProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Image, InstagramMessageDto>()
                .ForMember(x => x.Name, x => x.UseValue(Guid.NewGuid().ToString()));
            CreateMap<Template, TemplateViewModel>()
           .ConvertUsing(x => new TemplateViewModel(x.Id, x.Name, FromTemplate(x), FromImage(x.Background), FromImage(x.Overlay), x.IsInstaPrinterTemplate));

            CreateMap<TemplateViewModel, Template>() 
                .ConvertUsing(FromTemplateViewModel); 

        }
        private IEnumerable<TemplateImageData> FromTemplate(Template template)
        {
            return template.Images.Select(c =>
                    new TemplateImageData() { X = c.X, Y = c.Y, Width = c.Width, Height = c.Height });
        }

        private ImageViewModel FromImage(FileData image)
        {
            return image.Return(x => new ImageViewModel(x.Id, "", x.Data), null);
        }

        private FileData FromImageViewModel(ImageViewModel image)
        {
            return image.Return(x => new FileData()
            {
                Id = x.Id,
                Data = x.Data
            }, null);
        }

     
        private Template FromTemplateViewModel(TemplateViewModel template)
        {
            var background = FromImageViewModel(template.Background);
            var overlay = FromImageViewModel(template.Overlay);

            return new Template()
            {
                Name = template.Name,
                Id = template.Id,
                Background = background,
                BackgroundId = background.Return(x => x.Id, (int?)null),
                Overlay = overlay,
                OverlayId = overlay.Return(x => x.Id, (int?)null),
                Images = template.TemplateImages.Select(x => new TemplateImage() { Height = x.Height, Width = x.Width, X = x.X, Y = x.Y }).ToList(),
            };
        }
    }
}
