using Microsoft.AspNetCore.Identity;

namespace Persistence.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; } = string.Empty;
    }
}
