using AutoMapper;
using BeeNetServer.Background;
using BeeNetServer.Background.PictureStore;
using BeeNetServer.Models;
using BeeNetServer.Response.Label;
using BeeNetServer.Response.Picture;
using System.Linq;

namespace BeeNetServer.Tool
{
    public class LibraryMappingProfile : Profile
    {
        public LibraryMappingProfile()
        {

            CreateMap<Picture, PictureGetResponse>()
                .ForMember(dest => dest.Labels, config =>
                   config.MapFrom(src => src.PictureLabels.Select(pl => pl.Label)));

            CreateMap<Label, LabelGetResponse>();
            CreateMap<Picture, PictureStorePicture>()
                .ForMember(dest => dest.Labels, config =>
                   config.MapFrom(src => src.PictureLabels.Select(pl => pl.LabelName)));
            CreateMap<Label, PictureStoreLabel>();
            CreateMap<PictureStorePicture, PictureStoreImportProgressPictureResult>();
            CreateMap<PictureStorePicture, Picture>();
        }
    }
}
