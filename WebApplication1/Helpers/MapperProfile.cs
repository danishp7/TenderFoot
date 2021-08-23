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
            CreateMap<JobApplicationDto, JobApplication>().ReverseMap();
        }
    }
}
