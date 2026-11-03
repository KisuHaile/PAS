using Application.Common.Mappings;
using AutoMapper;
using System;

namespace Application.Features.Users.Employees.Dtos
{
    public class EmployeeDetailDto : IMapFrom<Domain.Users.Employee>
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public UserAccountSummaryDto? UserAccount { get; set; }
        public List<EmployeeActivityDto> RecentActivities { get; set; } = new();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Users.Employee, EmployeeDetailDto>();
        }
    }

    public class EmployeeListDto
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool HasUserAccount { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }

    public class UserAccountSummaryDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class EmployeeActivityDto
    {
        public DateTime Date { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Entity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
