using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Repos;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class VacanciesController : ControllerBase
    {
        private readonly IVacancyRepo _repo;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _user;
        public VacanciesController(ILogger<VacanciesController> logger, IMapper mapper, UserManager<AppUser> userManager, IVacancyRepo vacancyRepo)
        {
            _repo = vacancyRepo;
            _logger = logger;
            _mapper = mapper;
            _user = userManager;
        }


        // POST: api/Vacancies
        [HttpPost("post")]
        public async Task<IActionResult> Post(VacancyDto vacancyDto)
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

                // check if vacancy with same name is already exist or not
                if (await _repo.IsVacancy(vacancyDto.Title))
                {
                    _logger.LogInformation("vacancy already exist...");
                    return BadRequest("vacancy already available for similar title.");
                }

                // validate vacancy model
                if (!await _repo.IsValidate(vacancyDto))
                {
                    _logger.LogInformation("vacancy model validation failed...");
                    return BadRequest("there is something wrong with model...");
                }

                // get the logged in user for adding into associates (i.e job vacancy)
                var user = await _user.FindByEmailAsync(User.Identity.Name);
                if (user == null)
                {
                    _logger.LogWarning("no such user exist in db...");
                    return BadRequest("something went wrong");
                }
                
                var vacancy = _mapper.Map<Vacancy>(vacancyDto);
                _repo.AddAssociates(ref vacancy, user);

                if (!await _repo.PostVacancy(vacancy))
                {
                    _logger.LogWarning("vacancy didn't save into db...");
                    return BadRequest("something went wrong...");
                }
                else
                {
                    var result = _mapper.Map<VacancyDto>(vacancy);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("something went wrong...");
            }
        }

        // GET: api/vacancies
        [HttpGet]
        public async Task<IActionResult> Get()
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
                    return BadRequest("Please signed in to your account.");
                }

                // check if user is in role
                if (!User.IsInRole(Role.Admin.ToString()))
                {
                    _logger.LogWarning("user is not in employer role to post vacancy");
                    return BadRequest("You do not have privilege to process this request");
                }

                // get the user so that all of his/her vacancies posted can be get. 
                var user = await _user.FindByEmailAsync(User.Identity.Name);
                if (user == null)
                {
                    _logger.LogWarning("no such user exist");
                    return BadRequest("something went wrong...");
                }
                
                // get all vacancies of admin
                var vacancies = await _repo.GetVacancies(user);
                if (vacancies == null)
                {
                    _logger.LogInformation("no vacancy posted yet by: " + user.UserName);
                    return NoContent();
                }

                var result = _mapper.Map<ICollection<Vacancy>, ICollection<VacancyDto>>(vacancies);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("something went wrong...");
            }
        }

    }
}
