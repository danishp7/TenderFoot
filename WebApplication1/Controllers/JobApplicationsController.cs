using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Repos;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly UserManager<AppUser> _user;
        private readonly IJobApplicationRepo _repo;
        public JobApplicationsController(ILogger<JobApplicationsController> logger, UserManager<AppUser> userManager,
                                         IJobApplicationRepo jobApplication)
        {
            _logger = logger;
            _user = userManager;
            _repo = jobApplication;
        }

        // GET /api/jobapplications
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

                // first check if user is authenticated (logged in) or not
                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("user is not logged in...");
                    return Unauthorized("Please signed in to your account.");
                }

                // check if user is in role
                if (!User.IsInRole(Role.Admin.ToString()))
                {
                    _logger.LogWarning("user is not in employer role to post vacancy");
                    return Unauthorized("You do not have privilege to process this request");
                }

                // get the applications against given id
                //await _repo.GetAllApplications(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("something went wrong...");
            }
        }
    }
}