using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Models;

namespace WebApplication1.Repos
{
    public class VacancyRepo : IVacancyRepo
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger _logger;
        public VacancyRepo(ApplicationDbContext context, ILogger<VacancyRepo> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        public async Task<bool> IsVacancy(string title)
        {
            try
            {
                var result = await _ctx.Vacancies.FirstOrDefaultAsync(v => v.Title == title);
                if (result == null)
                {
                    _logger.LogInformation("No vacancy of title: " + title + " exists");
                    return false;
                }
                else
                {
                    _logger.LogInformation("Vacancy of title: " + title + " exists");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> PostVacancy(Vacancy vacancy)
        {
            try
            {
                await _ctx.Vacancies.AddAsync(vacancy);
                return  await _ctx.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public Task<bool> IsValidate(VacancyDto vacancyDto)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(vacancyDto.Title) || string.IsNullOrEmpty(vacancyDto.Description) ||
                        string.IsNullOrEmpty(vacancyDto.Type) || string.IsNullOrEmpty(vacancyDto.MinimumRequirement) ||
                        string.IsNullOrEmpty(vacancyDto.Responsibility) || string.IsNullOrEmpty(vacancyDto.Salary))
                    {
                        _logger.LogInformation("vacancy state is not valid.. some of props have null values. \n");
                        _logger.LogInformation("title: " + vacancyDto.Title + Environment.NewLine);
                        _logger.LogInformation("description: " + vacancyDto.Description + Environment.NewLine);
                        _logger.LogInformation("type: " + vacancyDto.Type + Environment.NewLine);
                        _logger.LogInformation("minimum requirement: " + vacancyDto.MinimumRequirement + Environment.NewLine);
                        _logger.LogInformation("responsibility: " + vacancyDto.Responsibility + Environment.NewLine);
                        _logger.LogInformation("salary: " + vacancyDto.Salary + Environment.NewLine);

                        return false;
                    }
                    else
                    {
                        _logger.LogInformation("vacancy state is valid.. " + Environment.NewLine);
                        _logger.LogInformation("title: " + vacancyDto.Title + Environment.NewLine);
                        _logger.LogInformation("description: " + vacancyDto.Description + Environment.NewLine);
                        _logger.LogInformation("type: " + vacancyDto.Type + Environment.NewLine);
                        _logger.LogInformation("minimum requirement: " + vacancyDto.MinimumRequirement + Environment.NewLine);
                        _logger.LogInformation("responsibility: " + vacancyDto.Responsibility + Environment.NewLine);
                        _logger.LogInformation("salary: " + vacancyDto.Salary + Environment.NewLine);

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return false;
                }
            });
        }

        public void AddAssociates(ref Vacancy vacancy, AppUser user)
        {
            try
            {
                JobVacancy jobVacancy = new JobVacancy()
                {
                    Vacancy = vacancy,
                    AppUser = user
                };
                vacancy.JobVacancies = new List<JobVacancy>();
                vacancy.JobVacancies.Add(jobVacancy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                vacancy = null;
            }
        }

        public async Task<ICollection<Vacancy>> GetVacancies(AppUser user)
        {
            try
            {
                var vacancies = await _ctx.JobVacancies
                                            .Include(v => v.Vacancy)
                                            .Where(v => v.AppUser == user)
                                            .Select(v => v.Vacancy)
                                            .ToListAsync();
                if (vacancies.Count == 0)
                {
                    _logger.LogInformation("no vacancy posted by the user: " + user.UserName);
                    return null;
                }
                return vacancies;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<int> IsVacancy(int id)
        {
            try
            {
                var result = await _ctx.Vacancies.FindAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("no vacancy exist with id: " + id);
                    return 0;
                }

                _logger.LogWarning("vacancy exist with id: " + id);
                return result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 0;
            }
        }
    }
}
