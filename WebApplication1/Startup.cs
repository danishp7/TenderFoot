using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApplication1.Data;
using WebApplication1.Helpers;
using WebApplication1.Middlewares;
using WebApplication1.Models;
using WebApplication1.Repos;

namespace WebApplication1
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.CorsConfiguration();

            // add odata
            services.AddOData();

            // add sql
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_config.GetConnectionString("DefaultConnectionString")));

            // add identity for app user
            services.AddIdentity<AppUser, IdentityRole>(cfg =>
            {
                // need to configure this
                cfg.SignIn.RequireConfirmedAccount = false;
                cfg.SignIn.RequireConfirmedEmail = false;
                // need to configure this

                cfg.User.RequireUniqueEmail = true;
                cfg.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_.0123456789-@";

                cfg.Password = new PasswordOptions
                {
                    RequireDigit = true,
                    RequiredLength = 5,
                    RequiredUniqueChars = 1,
                    RequireLowercase = true,
                    RequireUppercase = true,
                    RequireNonAlphanumeric = false
                };
                cfg.Lockout = new LockoutOptions
                {
                    DefaultLockoutTimeSpan = new TimeSpan(0, 0, 5, 0),
                    MaxFailedAccessAttempts = 5
                };
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            // remove auto referencing loop
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // add automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // add repos
            services.AddScoped<IVacancyRepo, VacancyRepo>();
            services.AddScoped<IEncryptRepo, EncryptRepo>();
            services.AddScoped<IApplicationRepo, ApplicationRepo>();
            services.AddTransient<SeedData>();

            // add application settings service
            services.Configure<ApplicationSettings>(_config.GetSection("ApplicationSettings"));

            // add jwt authentication
            // add authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // add cors for react app to hit the endpoints of this server
            services.AddCors();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // use cors
            app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseValidateJWT();

            app.UseEndpoints(endpoints =>
            {
                // for odata
                //endpoints.EnableDependencyInjection(); 
                endpoints.MapControllers();
                // mapping routes for mvc
                //endpoints.MapControllerRoute(name: "Default", pattern: "[Controller=Home]/[Action=Index]/{id?}"); 
            });
        }
    }
}
