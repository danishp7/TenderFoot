using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Repos
{
    public interface IApplicationRepo
    {
        Task<ApplicationDto> IsValidApplication(ApplicationDto applicationDto);
        Task<bool> SaveApplication(ApplicationDto applicationDto, AppUser user, int vacancyId);
        Task<bool> IsApplication(int vacancyId, string id);
    }
}
