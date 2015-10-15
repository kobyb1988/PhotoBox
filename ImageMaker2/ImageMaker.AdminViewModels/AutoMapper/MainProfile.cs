using System;
using System.IO;
using System.Linq;
using System.Monads;
using AutoMapper;
using ImageMaker.AdminViewModels.ViewModels;
using ImageMaker.AdminViewModels.ViewModels.Images;
using ImageMaker.Common.Dto;
using ImageMaker.CommonViewModels.ViewModels.Images;
using ImageMaker.CommonViewModels.ViewModels.Settings;
using ImageMaker.Entities;

namespace ImageMaker.AdminViewModels.AutoMapper
{
    public class MainProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<CameraSettingsExplorerViewModel, CameraSettingsDto>();
            CreateMap<AppSettingsExplorerViewModel, AppSettingsDto>()
                .ForMember(x => x.DateEnd, x => x.ResolveUsing(c => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, c.DateEnd.Hours, c.DateEnd.Minutes, 0)))
                .ForMember(x => x.DateStart, x => x.ResolveUsing(c => new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, c.DateStart.Hours, c.DateStart.Minutes, 0)));
            CreateMap<ThemeManagerViewModel, ThemeSettingsDto>()
               // .ForMember(x => x.BackgroundImage, x => x.MapFrom(d => d.MainWindowImage.With(c => c.Data)))
                .ForMember(x => x.MainBackgroundColor, x => x.MapFrom(d => d.MainWindowBackgroundColor))
                .ForMember(x => x.MainBorderColor, x => x.MapFrom(d => d.MainWindowBorderColor))
                .ForMember(x => x.MainForegroundColor, x => x.MapFrom(d => d.MainWindowForegroundColor))
                .ForMember(x => x.OtherBackgroundColor, x => x.MapFrom(d => d.OtherWindowsBackgroundColor))
                .ForMember(x => x.OtherBorderColor, x => x.MapFrom(d => d.OtherWindowsBorderColor))
                .ForMember(x => x.OtherForegroundColor, x => x.MapFrom(d => d.OtherWindowsForegroundColor));

            CreateMap<Template, TemplateViewModel>()
                .ConvertUsing(FromTemplate);

            CreateMap<TemplateViewModel, Template>()
                .ConvertUsing(FromTemplateViewModel);

            CreateMap<Image, ImageViewModel>()
                .ConvertUsing(FromImage);
            //CreateMap<Composition, CompositionViewModel>()
            //    .ConvertUsing(x => new CompositionViewModel(x.Name, x.Id, FromTemplate(x.Template), FromImage(x.Background), FromImage(x.Overlay)));

            //CreateMap<CompositionViewModel, Composition>()
            //    .ConvertUsing(FromCompositionViewModel);
        }

        private TemplateViewModel FromTemplate(Template template)
        {
            return new TemplateViewModel(template.Name, (uint) template.Width, (uint) template.Height, template.Id,
                template.Images.Select(c =>
                    new TemplateImageViewModel( c.X, c.Y,  c.Width, c.Height, c.Id, template.Width, template.Height)), 
                    template.Background.With(FromImage), 
                    template.Overlay.With(FromImage));
        }

        private ImageViewModel FromImage(FileData image)
        {
            return image.Return(x => new ImageViewModel(x.Id, "", x.Data), null);
        }

        private ImageViewModel FromImage(Image image)
        {
            if (!File.Exists(image.Path))
                return null;

            byte[] data = File.ReadAllBytes(image.Path);

            return new ImageViewModel(image.Id, image.Name, data);
        }

        //private ImageViewModel FromImage(Image image)
        //{
        //    return image.Return(x => new ImageViewModel(x.Id, x.Name, x.Data.Data), null);
        //}

        //private Image FromImageViewModel(ImageViewModel image)
        //{
        //    return image.Return(x => new Image()
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Data = new FileData()
        //        {
        //            Id = x.Id,
        //            Data = x.Data
        //        }
        //    }, null);
        //}

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
                Id = template.Id,
                Height = (int) template.Height,
                Width = (int) template.Width,
                Name = template.Name,
                Images = template.Children.Select(c => new TemplateImage()
                {
                    Width = c.Width,
                    Height =  c.Height,
                    X =  c.X,
                    Y =  c.Y,
                    Id = c.Id
                }).ToList(),

                Background = background,
                //BackgroundId = background.Return(x => x.Id, (int?)null),
                Overlay = overlay,
                //OverlayId = overlay.Return(x => x.Id, (int?)null),
            };
        }

        //private Composition FromCompositionViewModel(CompositionViewModel composition)
        //{
        //    var background = FromImageViewModel(composition.Background);
        //    var overlay = FromImageViewModel(composition.Overlay);

        //    return new Composition()
        //    {
        //        Name = composition.Name,
        //        Id = composition.Id,
        //        Background = background,
        //        BackgroundId = background.Return(x => x.Id, (int?) null),
        //        Overlay = overlay,
        //        OverlayId = overlay.Return(x => x.Id, (int?) null),
        //        TemplateId = composition.Template.Id
        //    };
        //}
    }
}
