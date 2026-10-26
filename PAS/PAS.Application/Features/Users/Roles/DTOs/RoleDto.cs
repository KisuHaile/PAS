namespace Application.Features.Users.Roles.Dtos;

public class RoleDto
{
    public Guid Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public List<string> Permissions { get; set; } = new();

}

public class RoleDetailDto : RoleDto
{
    public List<RoleUserDto> Users { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class RoleUserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UserRoleDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public List<RoleAssignmentDto> Roles { get; set; } = new();
}

public class RoleAssignmentDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsAssigned { get; set; }
}