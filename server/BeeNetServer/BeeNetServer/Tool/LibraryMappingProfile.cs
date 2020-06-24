using AutoMapper;
using BeeNetServer.Dto;
using BeeNetServer.Models;
using System.Linq;

namespace BeeNetServer.Tool
{
    public class LibraryMappingProfile : Profile
    {
        public LibraryMappingProfile()
        {
            CreateMap<Picture, PictureResponseDto>()
                .ForMember(dest => dest.PictureLabels,
                config => config.MapFrom(src =>
                    src.PictureLabels.Select(r => new PictureLabelResponseDto
                    {
                        LabelName = r.Label.Name,
                        Color = r.Label.Color,
                        Num = r.Label.Num
                    }).ToList()));
            CreateMap<LabelDto, Label>();
        }
    }
}
