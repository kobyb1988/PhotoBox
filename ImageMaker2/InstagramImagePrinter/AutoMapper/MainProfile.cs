using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ImageMaker.MessageQueueing.Dto;
using ImageMaker.WebBrowsing;

namespace InstagramImagePrinter.AutoMapper
{
    public class MainProfile : Profile
    {
        protected override void Configure()
        {
            CreateMap<Image, InstagramMessageDto>()
                .ForMember(x => x.Name, x => x.UseValue(Guid.NewGuid().ToString()));
        }
    }
}
