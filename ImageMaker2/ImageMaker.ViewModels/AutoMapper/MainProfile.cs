using System.Collections.Generic;
using System.Linq;
using System.Monads;
using AutoMapper;
using ImageMaker.CommonViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Entities;
using ImageMaker.MessageQueueing.Dto;
using ImageMaker.ViewModels.ViewModels.Images;

namespace ImageMaker.ViewModels.AutoMapper
{
    public class MainProfile : Profile
    {
        protected override void Configure()
        {
            //CreateMap<Composition, CompositionViewModel>()
            //    .ConvertUsing(x => new CompositionViewModel(x.Id, x.TemplateId, x.Name, FromTemplate(x.Template), FromImage(x.Background), FromImage(x.Overlay)));

            CreateMap<Template, TemplateViewModel>()
                .ConvertUsing(x => new TemplateViewModel(x.Id, x.Name, FromTemplate(x), FromImage(x.Background), FromImage(x.Overlay)));

            CreateMap<TemplateViewModel, Template>()
                .ConvertUsing(FromTemplateViewModel);

            CreateMap<ImageViewModel, Image>()
                .ConvertUsing(ImageFromImageViewModel);

            //CreateMap<PatternData, PatternDataViewModel>()
            //    .ConstructUsing(x => new PatternDataViewModel(x.Name, (int) x.PatternType, x.Data))
            //    .ReverseMap()
            //    .ForMember(x => x.PatternType, x => x.MapFrom(c => (PatternType) c.PatternType));

            //CreateMap<Pattern, PatternViewModel>()
            //    .ConstructUsing(x => new PatternViewModel(x.Name, (int) x.PatternType))
            //    .ReverseMap();

            CreateMap<InstagramMessageDto, InstagramImageViewModel>()
                .ConvertUsing(x => new InstagramImageViewModel(x.Data, x.Width, x.Height, x.Name));
        }

        private IEnumerable<TemplateImageData> FromTemplate(Template template)
        {
            return template.Images.Select(c =>
                    new TemplateImageData() { X = c.X,  Y = c.Y, Width = c.Width, Height = c.Height});
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

        private Image ImageFromImageViewModel(ImageViewModel image)
        {
            return image.Return(x => new Image()
            {
                Id = x.Id,
                Name = x.Name,
                Data = new FileData()
                {
                    Id = x.Id,
                    Data = x.Data
                }
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
