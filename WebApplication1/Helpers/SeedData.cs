using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Enums;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class SeedData
    {
        private readonly ILogger<SeedData> _logger;
        private readonly ApplicationDbContext _ctx;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedData(ILogger<SeedData> logger, ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _ctx = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedRoles()
        {
            _ctx.Database.EnsureCreated();

            // create roles before hand
            if (!_roleManager.Roles.Any())
            {
                // creating roles
                var hrRole = new IdentityRole
                {
                    Name = Role.Admin.ToString(),
                    NormalizedName = Role.Admin.ToString()
                };
                var staffRole = new IdentityRole
                {
                    Name = Role.AppUser.ToString(),
                    NormalizedName = Role.AppUser.ToString()
                };
                var isEmployerRole = await _roleManager.CreateAsync(hrRole);
                var isEmployeeRole = await _roleManager.CreateAsync(staffRole);

                if (isEmployeeRole.Succeeded && isEmployerRole.Succeeded)
                    _logger.LogInformation("Roles created successfully!");
                else
                    _logger.LogInformation("Roles creation failed!");
            }
        }
    }
}
