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
    public class JobApplicationRepo : IJobApplicationRepo
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ILogger _logger;
        public JobApplicationRepo(ApplicationDbContext context, ILogger<JobApplicationRepo> logger)
        {
            _ctx = context;
            _logger = logger;
        }
        public async Task<ICollection<JobApplication>> GetAllApplications(int id)
        {
            try
            {
                var applications = await _ctx.JobApplications.Where(_ => _.VacancyId == id).Include(user => user.AppUser).ToListAsync();
                if (applications == null)
                {
                    _logger.LogWarning("no applications received for vacancy: " + id);
                    return null;
                }
                _logger.LogWarning("applications received for vacancy: " + id + ", are: " + applications.Count);
                return applications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
