using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Persistence.Identity;

namespace Persistence.Seed
{
    public static class DataSeed
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            if (context.Database.IsRelational())
                await context.Database.MigrateAsync();

            await CategorySeed.SeedAsync(context);

            await RoleSeed.SeedAsync(roleManager);

            await DefaultUserSeed.SeedAsync(userManager);

            await BusinessDataSeed.SeedAsync(context);
        }
    }
}
