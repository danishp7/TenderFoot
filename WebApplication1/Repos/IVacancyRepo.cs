using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Repos
{
    public interface IVacancyRepo
    {
        Task<bool> PostVacancy(Vacancy vacancy);
        Task<bool> IsVacancy(string title);
        Task<bool> IsValidate(VacancyDto vacancyDto);
        void AddAssociates(ref Vacancy vacancy, AppUser user);
        Task<ICollection<Vacancy>> GetVacancies(AppUser user);
    }
}
