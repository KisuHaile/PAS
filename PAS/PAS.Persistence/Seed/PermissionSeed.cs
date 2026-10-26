using Microsoft.AspNetCore.Identity;

namespace Persistence.Seed
{
    public static class PermissionSeed
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");

            if (adminRole != null)
            {
                var claims = await roleManager.GetClaimsAsync(adminRole);

                if (!claims.Any())
                {
                    await roleManager.AddClaimAsync(adminRole,
                        new System.Security.Claims.Claim("Permission", "FullAccess"));
                }
            }
        }
    }
}
