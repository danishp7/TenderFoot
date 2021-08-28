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
    public class ApplicationsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly UserManager<AppUser> _user;
        private readonly SignInManager<AppUser> _signInUser;
        private readonly IApplicationRepo _repo;
        private readonly IVacancyRepo _vacancyRepo;

        public ApplicationsController(ILogger<ApplicationsController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                                      IApplicationRepo applicationRepo, IVacancyRepo vacancyRepo)
        {
            _logger = logger;
            _user = userManager;
            _signInUser = signInManager;
            _repo = applicationRepo;
            _vacancyRepo = vacancyRepo;
        }

        // POST // api/applications/
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id, IFormFile resume, IFormFile coverLetter)
        {
            try
            {
                // check model state
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("model state is not valid for this request...");
                    return BadRequest("something wrong with the request...");
                }

                // check if both files have values or null
                if (resume == null || coverLetter == null)
                {
                    _logger.LogWarning("one or both files are null");
                    return BadRequest("something went wrong");
                }

                // check if that vacancy exist or not
                var vacancyId = await _vacancyRepo.IsVacancy(id);
                if (vacancyId == 0)
                {
                    _logger.LogWarning("no such vacancy exist with id: " + id);
                    return BadRequest("something went wrong");
                }

                // check if user has already applied for this vacancy or not
                // get the logged in user
                var user = await _user.FindByEmailAsync(User.Identity.Name);

                if (await _repo.IsApplication(vacancyId, user.Id))
                {
                    _logger.LogWarning("you have already applied for this vacancy: " + vacancyId);
                    return BadRequest("you have already applied for this vacancy...");
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
                if (!User.IsInRole(Role.AppUser.ToString()))
                {
                    _logger.LogWarning("user is not in employer role to post vacancy");
                    return Unauthorized("You do not have privilege to process this request");
                }

                // check if application is in valid state
                ApplicationDto applicationDto = new ApplicationDto
                {
                    Resume = resume,
                    CoverLetter = coverLetter
                };
                if (_repo.IsValidApplication(applicationDto) == null)
                {
                    _logger.LogWarning("application is not valid...");
                    return BadRequest("something went wrong");
                }
                
                // save the application
                if (!await _repo.SaveApplication(applicationDto, user, vacancyId))
                {
                    _logger.LogWarning("application cannot be saved successfully into db...");
                    return BadRequest("something went wrong while saving the application");
                }
                return Ok("you have applied for this vacancy! thanks!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("something went wrong ... ");
            }
        }

    }
}