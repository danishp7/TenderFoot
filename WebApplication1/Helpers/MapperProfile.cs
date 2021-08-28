using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserDto, AppUser>().ReverseMap();
            CreateMap<VacancyDto, Vacancy>().ReverseMap();
            CreateMap<JobApplication, JobApplicationDto>()
                .ForPath(m => m.UserName, dst => dst.MapFrom(_ => _.AppUser.Email))
                .ReverseMap();
        }
    }
}
