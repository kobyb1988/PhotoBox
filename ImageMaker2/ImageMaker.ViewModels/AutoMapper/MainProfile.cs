using System;
using System.Collections.Generic;
using System.Linq;
using System.Monads;
using AutoMapper;
using AutoMapper.Mappers;
using ImageMaker.CommonViewModels;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.Entities;
using ImageMaker.MessageQueueing.Dto;
using ImageMaker.PatternProcessing.Dto;
using ImageMaker.ViewModels.ViewModels.Images;
using ImageMaker.ViewModels.ViewModels.Patterns;
using CompositionViewModel = ImageMaker.ViewModels.ViewModels.Images.CompositionViewModel;

namespace ImageMaker.ViewModels.AutoMapper
{
    public class MainProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Composition, CompositionViewModel>()
                .ConvertUsing(x => new CompositionViewModel(x.Id, x.TemplateId, x.Name, FromTemplate(x.Template), FromImage(x.Background), FromImage(x.Overlay)));

            CreateMap<CompositionViewModel, Composition>()
                .ConvertUsing(FromCompositionViewModel);

            CreateMap<PatternData, PatternDataViewModel>()
                .ConstructUsing(x => new PatternDataViewModel(x.Name, (int) x.PatternType, x.Data))
                .ReverseMap()
                .ForMember(x => x.PatternType, x => x.MapFrom(c => (PatternType) c.PatternType));

            CreateMap<Pattern, PatternViewModel>()
                .ConstructUsing(x => new PatternViewModel(x.Name, (int) x.PatternType))
                .ReverseMap();

            CreateMap<InstagramMessageDto, InstagramImageViewModel>()
                .ConvertUsing(x => new InstagramImageViewModel(x.Data, x.Width, x.Height, x.Name));
        }

        private IEnumerable<TemplateImageData> FromTemplate(Template template)
        {
            return template.Images.Select(c =>
                    new TemplateImageData() { X = c.X,  Y = c.Y, Width = c.Width, Height = c.Height});
        }

        private ImageViewModel FromImage(Image image)
        {
            return image.Return(x => new ImageViewModel(x.Id, x.Name, x.Data.Data), null);
        }

        private Image FromImageViewModel(ImageViewModel image)
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

        private Composition FromCompositionViewModel(CompositionViewModel composition)
        {
            var background = FromImageViewModel(composition.Background);
            var overlay = FromImageViewModel(composition.Overlay);

            return new Composition()
            {
                Name = composition.Name,
                Id = composition.Id,
                Background = background,
                BackgroundId = background.Return(x => x.Id, (int?)null),
                Overlay = overlay,
                OverlayId = overlay.Return(x => x.Id, (int?)null),
                Template = new Template() { Images = composition.TemplateImages.Select(x => new TemplateImage() { Height = x.Height, Width = x.Width, X = x.X, Y = x.Y }).ToList()},
                TemplateId = composition.TemplateId
            };
        }
    }
}
