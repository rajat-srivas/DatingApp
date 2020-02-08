using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, options => {
                    options.MapFrom(source => source.Photos.FirstOrDefault(p => p.isMain).Url);
                })
                .ForMember(dest => dest.Age, options => {
                    options.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetails>()  
                .ForMember(dest => dest.PhotoUrl, options => {
                    options.MapFrom(source => source.Photos.FirstOrDefault(p => p.isMain).Url);
                })
                 .ForMember(dest => dest.Age, options => {
                    options.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotosForDetailDto>();
            CreateMap<UserForEdit, User>();
             CreateMap<Photo, PhotoForReturnDto>();
              CreateMap<PhotoForCreationDto, Photo>();
              CreateMap<UserForRegister, User>();
            
        }
    }
}