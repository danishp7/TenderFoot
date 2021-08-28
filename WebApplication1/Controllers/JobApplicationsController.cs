using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Repos;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "AppUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _user;
        private readonly SignInManager<AppUser> _signInUser;
        private readonly IJobApplicationRepo _repo;
        private readonly IVacancyRepo _vacancyRepo;
        public JobApplicationsController(ILogger<JobApplicationsController> logger, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                         IJobApplicationRepo jobApplication, IVacancyRepo vacancyRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _user = userManager;
            _signInUser = signInManager;
            _repo = jobApplication;
            _vacancyRepo = vacancyRepo;
        }

        // GET /api/jobapplications/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                // check model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("model state is not valid for this request...");
                    return BadRequest("something wrong with the request...");
                }

                // check if vacancy even exist or not
                // check if that vacancy exist or not
                var vacancyId = await _vacancyRepo.IsVacancy(id);
                if (vacancyId == 0)
                {
                    _logger.LogWarning("no such vacancy exist with id: " + id);
                    return BadRequest("something went wrong");
                }

                // if user logged in or not
                if (!_signInUser.IsSignedIn(User))
                {
                    _logger.LogWarning("user is not logged in...");
                    return Unauthorized("Please signed in to your account.");
                }

                // first check if user is authenticated or not
                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("user is not authenticated...");
                    return Unauthorized("Please signed in to your account.");
                }

                // check if user is in role
                //if (!User.IsInRole(Role.Admin.ToString()))
                //{
                //    _logger.LogWarning("user is not in employer role to post vacancy");
                //    return Unauthorized("You do not have privilege to process this request");
                //}

                // get applications against given id
                var apps = await _repo.GetAllApplications(vacancyId);
                if (apps == null)
                {
                    _logger.LogWarning("no application received for vacancy: " + vacancyId);
                    return Ok(null);
                }
                var applications = _mapper.Map<ICollection<JobApplication>, ICollection<JobApplicationDto>>(apps);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("something went wrong...");
            }
        }
    }
}