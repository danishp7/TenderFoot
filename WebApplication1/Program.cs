using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Data;
using WebApplication1.Helpers;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            //for seeding data
            //var host = CreateHostBuilder(args);
            //SeedDb(host);
            //host.Run();
        }

        public static void SeedDb(IWebHost host)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            var _logger = host.Services.GetService<ILogger<Program>>();
            using (var scope = scopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // get the required parameters for the method
                    var _ctx = services.GetRequiredService<ApplicationDbContext>();

                    // add the migrations
                    _ctx.Database.Migrate();

                    // now get the seeder from transient service
                    var seed = services.GetRequiredService<SeedData>();
                    seed.SeedRoles().Wait();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to insert data into db...", ex);
                }
            };
        }
        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        //for seeding data
        //public static IWebHost CreateHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>()
        //        .Build();
    }
}
