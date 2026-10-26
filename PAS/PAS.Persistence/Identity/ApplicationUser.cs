using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string? Position { get; set; }

        [Column("PhotoUrl")]
        public string? ProfileImageUrl { get; set; }
    }
}
