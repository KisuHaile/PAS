using Microsoft.AspNetCore.Identity;

namespace Persistence.Identity
{
    public class IdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Admin",
                    Description = "System Administrator"
                });

                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Manager",
                    Description = "System Manager"
                });

                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Employee",
                    Description = "Normal Employee"
                });
            }

            if (!userManager.Users.Any())
            {
                var admin = new ApplicationUser
                {
                    FullName = "System Admin",
                    UserName = "admin",
                    Email = "admin@pms.com",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, "Admin@123");

                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
