using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Diplom.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleInitializer.InitializeAsync(rolesManager);
                }
                catch(Exception ex)
                {
                    Log.ForContext<Program>().Error(ex, "An error occurred while seeding the database.");
                }
            }

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .ConfigureLogging(config => { config.ClearProviders(); })
                          .UseSerilog((hostingContext, loggerConfiguration) =>
                          {
                              loggerConfiguration
                                      .ReadFrom.Configuration(hostingContext.Configuration);
                          })
                          .UseStartup<Startup>();
        }
    }
}