using System.Threading.Tasks;
using Diplom.Common;
using Microsoft.AspNetCore.Identity;

namespace Diplom.Server
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            var isDirectorExists = await roleManager.RoleExistsAsync(RoleNames.Director);
            var isWorkerExists = await roleManager.RoleExistsAsync(RoleNames.Worker);
            var isUserExists = await roleManager.RoleExistsAsync(RoleNames.User);

            if(!isDirectorExists)
            {
                var directorRole = new IdentityRole(RoleNames.Director);
                await roleManager.CreateAsync(directorRole);
            }

            if(!isWorkerExists)
            {
                var workerRole = new IdentityRole(RoleNames.Worker);
                await roleManager.CreateAsync(workerRole);
            }

            if(!isUserExists)
            {
                var userRole = new IdentityRole(RoleNames.User);
                await roleManager.CreateAsync(userRole);
            }
        }
    }
}