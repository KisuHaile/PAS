using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAS.Application.Features.Users.Authentication.DTOs
{
    public class AuthResultDto
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = new();
        public string[] Errors { get; set; } = Array.Empty<string>();
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public string? JoinDate { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string[] Permissions { get; set; } = Array.Empty<string>();
    }
}
