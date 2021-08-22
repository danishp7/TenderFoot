using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Helpers;
using WebApplication1.Models;

namespace WebApplication1.Repos
{
    public class ApplicationRepo : IApplicationRepo
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationSettings _appOptions;
        public ApplicationRepo(ApplicationDbContext context, ILogger<ApplicationRepo> logger,
                               IOptionsSnapshot<ApplicationSettings> options, IConfiguration configuration)
        {
            _ctx = context;
            _logger = logger;
            this._config = configuration;
            _appOptions = options.Value;
        }

        public Task<ApplicationDto> IsValidApplication(ApplicationDto applicationDto)
        {
            try
            {
                var task = Task.Run(() =>
                {
                    if (applicationDto.Resume == null || applicationDto.CoverLetter == null)
                        applicationDto = null;

                    if (applicationDto.Resume.Length == 0 || applicationDto.CoverLetter.Length == 0)
                        applicationDto = null;

                    if (applicationDto.Resume.Length > _appOptions.MaxBytes || applicationDto.CoverLetter.Length > _appOptions.MaxBytes)
                        applicationDto = null;

                    if (!_appOptions.AcceptedTypes.Any(_ => _ == Path.GetExtension(applicationDto.Resume.FileName)) ||
                        !_appOptions.AcceptedTypes.Any(_ => _ == Path.GetExtension(applicationDto.CoverLetter.FileName)))
                        applicationDto = null;

                    return applicationDto;

                });
                
                return task;
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

        public async Task<bool> SaveApplication(ApplicationDto applicationDto, AppUser user, int vacancyId)
        {
            try
            {
                // set file name and save to directory
                var files = await SaveFiles(user.Id, applicationDto, vacancyId);
                if (!files.Any())
                {
                    _logger.LogInformation("files are not being saved into directory...");
                    return false;
                }

                // set the associate relation
                JobApplication jobApplication = new JobApplication()
                {
                    AppUser = user,
                    Vacancy = await _ctx.Vacancies.FirstOrDefaultAsync(_ => _.Id == vacancyId),
                    Status = ApplicationStatus.Apply.ToString(),
                    Resume = files[0],
                    CoverLetter = files[1]
                };
                _ctx.JobApplications.Add(jobApplication);
                return await _ctx.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        private async Task<List<string>> SaveFiles(string userId, ApplicationDto applicationDto, int vacancyId)
        {
            try
            {
                List<string> files = new List<string>();
                var path = Path.Combine(_config.GetSection("FileSettings:Path").Value + userId + "/" + vacancyId);
                _logger.LogInformation("path for storing resumes: " + path);

                // create directory if doesn't exists
                // we don't want to create this folder manually in deploying
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // unique file name with same extension as valid uploaded file
                var resumeFileName = Guid.NewGuid().ToString() + Path.GetExtension(applicationDto.Resume.FileName);
                _logger.LogInformation("resume path: " + resumeFileName);

                var coverLetterFileName = Guid.NewGuid().ToString() + Path.GetExtension(applicationDto.CoverLetter.FileName);
                _logger.LogInformation("cover letter path: " + coverLetterFileName);

                // set path for this file
                var resumeFilePath = Path.Combine(path, resumeFileName);
                var coverLetterFilePath = Path.Combine(path, coverLetterFileName);

                // now use stream to read the file and store inside folder
                using (var stream = new FileStream(resumeFilePath /*path if file*/, FileMode.Create /*mode of file*/))
                {
                    await applicationDto.Resume.CopyToAsync(stream); // now file read and stored inside folder.
                }
                using (var stream = new FileStream(coverLetterFilePath /*path if file*/, FileMode.Create /*mode of file*/))
                {
                    await applicationDto.CoverLetter.CopyToAsync(stream); // now file read and stored inside folder.
                }
                files.Add(resumeFilePath);
                files.Add(coverLetterFilePath);

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<bool> IsApplication(int vacancyId, string id)
        {
            try
            {
                var result = await _ctx.JobApplications.FirstOrDefaultAsync(_ => _.AppUserId == id && _.VacancyId == vacancyId);
                if (result != null)
                {
                    _logger.LogWarning("there is already a record against vacancy id: " + vacancyId + ", with user: " + id);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
