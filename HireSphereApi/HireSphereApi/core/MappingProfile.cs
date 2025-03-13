using AutoMapper;
using HireSphereApi.api;
using HireSphereApi.api.Models;
using HireSphereApi.core.DTO;
using HireSphereApi.core.entities;
using HireSphereApi.entities;


namespace HireSphereApi.core
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<UserEntity, UserPostModel>().ReverseMap();
            CreateMap<UserEntity, UserDto>().ReverseMap();

            CreateMap<ExtractedDataEntity, ExtractedDataPostModel>().ReverseMap();
            CreateMap<ExtractedDataEntity, ExtractedDataDto>().ReverseMap();

            CreateMap<FileEntity,FilesPostModel>().ReverseMap();
            CreateMap<FileEntity, FileDto>().ReverseMap();

            CreateMap<ExtractedDataDto, ExtractedDataEntity>().ReverseMap();
        }
    }
}
