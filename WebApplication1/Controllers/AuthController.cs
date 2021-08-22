using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Dtos;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Repos;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _user;
        private readonly SignInManager<AppUser> _signInUser;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEncryptRepo _encryptRepo;
        private readonly IConfiguration _config;
        public AuthController(ILogger<AuthController> logger, IMapper mapper, IConfiguration configuration,
                              UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                              RoleManager<IdentityRole> roleManager, IEncryptRepo encryptRepo)
        {
            _logger = logger;
            _mapper = mapper;
            _user = userManager;
            _signInUser = signInManager;
            _roleManager = roleManager;
            _encryptRepo = encryptRepo;
            _config = configuration;
        }

        // Post: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Something went wrong...");

                // set password
                var userPassKey = _encryptRepo.CreatePasswordWithEncryption(userDto.Password);
                AppUser user = new AppUser
                {
                    UserName = userDto.Email,
                    Email = userDto.Email,
                    NormalizedUserName = userDto.Email.ToUpper().Normalize(),
                    NormalizedEmail = userDto.Email.ToUpper().Normalize(),
                    PasswordHash = userDto.Password,

                    // setting password byte[] and its key
                    HashPassword = userPassKey[0],
                    Key = userPassKey[1],
                };

                var isUserEmail = await _user.FindByEmailAsync(user.Email);
                if (isUserEmail != null)
                    return BadRequest("User exist with this email address. kindly provide different email id.");

                var isUserAdded = await _user.CreateAsync(user, userDto.Password);
                if (isUserAdded.Succeeded)
                {
                    var role = _roleManager.Roles.Where(r => r.Name == Role.AppUser.ToString()).FirstOrDefault();
                    if (role != null)
                        await _user.AddToRoleAsync(user, Role.AppUser.ToString());

                    else
                        _logger.LogWarning("No such role exist, user registered without role assigned to it");
                }
                else
                    return BadRequest("Something went wrong...");

                var newUser = _mapper.Map<UserDto>(user);
                return Created("api/auth/" + user.Id, newUser.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Something went wrong while processing request!");
            }
        }

        // Post: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Model state is invalid!");

                // check of this user is already logged in or not
                var user = await _user.FindByNameAsync(userDto.Email);
                if (user == null)
                    return BadRequest("No such user exist, please signup!");

                // first check if user is authenticated (logged in) or not
                
                if (_signInUser.IsSignedIn(User))
                {
                    _logger.LogInformation("user is already logged in...");
                    return Ok("you have already logged in...");
                }

                var isPassword = await _user.CheckPasswordAsync(user, userDto.Password);
                if (isPassword == false)
                {
                    _logger.LogWarning("Incorrect UserName or Password.");
                    return BadRequest("Incorrect UserName or Password.");
                }

                var isCustomPassword = await _encryptRepo.IsPassword(userDto.Password, user.HashPassword, user.Key);
                if (isPassword == false)
                {
                    _logger.LogWarning("Incorrect UserName or custom Password.");
                    return BadRequest("Incorrect UserName or Password.");
                }
                var isLogin = await _signInUser.PasswordSignInAsync(userDto.Email, userDto.Password, false, false);
                if (isLogin.Succeeded)
                {
                    _logger.LogInformation("Logged in, now returning token");
                    var role = await _user.GetRolesAsync(user);
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, role[0])
                    };

                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.Now.AddMinutes(int.Parse(_config.GetSection("AppSettings:ExpiryTime").Value)),
                        SigningCredentials = creds,
                        IssuedAt = DateTime.UtcNow,
                    };
                    _logger.LogInformation("token expires on: " + tokenDescriptor.Expires.ToString());
                    var tokenHandler = new JwtSecurityTokenHandler();

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    var returnUser = _mapper.Map<UserDto>(user);
                    return Ok(new
                    {
                        token = tokenHandler.WriteToken(token),
                        user = returnUser.Email,
                        message = "Login Successfully"
                    });
                }
                return BadRequest("Incorrect UserName or Password");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest("Something went wrong while processing request!");
            }
        }

        // Get: api/auth/logout
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInUser.SignOutAsync();
            return Ok("Logout Successfully!");
        }
    }
}