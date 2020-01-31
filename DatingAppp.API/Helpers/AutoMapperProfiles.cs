using System.Linq;
using AutoMapper;
using DatingAppp.API.Dtos;
using DatingAppp.API.Models;

namespace DatingAppp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.
                MapFrom(src =>src.Photos.FirstOrDefault(p => p.IsMain).Url))
                
                .ForMember(dest => dest.Age, opt => opt.
                MapFrom(a => a.DateOfBirth.CalculateAge()));


            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.
                MapFrom(src =>src.Photos.FirstOrDefault(p => p.IsMain).Url))

                .ForMember(dest => dest.Age, opt => opt.
                MapFrom(a => a.DateOfBirth.CalculateAge()));
            
            CreateMap<Photo, PhotoForDetailedDto>();

            CreateMap<UserForUpdateDto, User>();
            
        }
    }
}