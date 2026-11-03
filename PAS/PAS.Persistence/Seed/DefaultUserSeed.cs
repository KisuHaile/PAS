using Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Seed
{
    public static class DefaultUserSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@pms.com";

            var user = await userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
