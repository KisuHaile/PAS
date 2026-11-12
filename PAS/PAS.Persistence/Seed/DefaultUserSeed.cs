using Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Persistence.Seed
{
    public static class DefaultUserSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUsers = new[]
            {
                new { UserName = "admin", Email = "admin@pms.com", FullName = "System Administrator", Role = "Admin" },
                new { UserName = "manager", Email = "manager@pms.com", FullName = "Operations Manager", Role = "Manager" },
                new { UserName = "storekeeper", Email = "storekeeper@pms.com", FullName = "Head Storekeeper", Role = "StoreKeeper" },
                new { UserName = "employee", Email = "employee@pms.com", FullName = "Staff Employee", Role = "Employee" }
            };

            foreach (var u in defaultUsers)
            {
                var existing = await userManager.FindByEmailAsync(u.Email);
                if (existing == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = u.UserName,
                        Email = u.Email,
                        EmailConfirmed = true,
                        FullName = u.FullName,
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(user, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
            }
        }
    }
}
