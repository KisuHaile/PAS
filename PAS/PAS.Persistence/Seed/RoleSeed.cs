using Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Seed
{
    public static class RoleSeed
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles =
            {
                "Admin",
                "Manager",
                "StoreKeeper",
                "Employee"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }
        }
    }
}
