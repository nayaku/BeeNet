using AutoMapper;
using BeeNetServer.Dto;
using BeeNetServer.Models;
using BeeNetServer.Parameters;
using BeeNetServer.Response;
using System.Linq;

namespace BeeNetServer.Tool
{
    public class LibraryMappingProfile : Profile
    {
        public LibraryMappingProfile()
        {
            CreateMap<Picture, PictureResponseDto>()
                .ForMember(dest => dest.PictureLabels,
                config => config.MapFrom(
                    src => src.PictureLabels.Select(r => new PictureLabelResponseDto
                    {
                        LabelName = r.Label.Name,
                        Color = r.Label.Color,
                        Num = r.Label.Num
                    }))
                );
            CreateMap<LabelDto, Label>();
            CreateMap<Picture, ScreenShotResponseDto>();
            CreateMap<Workspace, WorkspaceListItemResponse>();
            CreateMap<Workspace, WorkspaceResponse>()
                .ForMember(dest => dest.Pictures, config => config.MapFrom(src =>
                src.WorkspacePictures.Select(p => p.Picture)));
            CreateMap<ScreenShot, PictureBase>();
            CreateMap<Workspace, WorkspaceDto>()
                .ForMember(dest => dest.Pictures, config => config.MapFrom(
                     src => src.WorkspacePictures.Select(wp => wp.Picture))
                );
            CreateMap<WorkspacePutParamters,Workspace>()
                .ForMember(dest=>dest.WorkspacePictures,config=>config.MapFrom(
                    src => src.PictureId.Select(id => new WorkspacePicture
                    {
                        PictureId = id,
                        WorkspaceName = src.Name
                    })


        }
    }
}
