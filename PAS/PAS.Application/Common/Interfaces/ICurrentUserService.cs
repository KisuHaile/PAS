namespace Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Username { get; }
        string? UserName => Username;
        string? EmployeeCode { get; }
        string? EmployeeName { get; }
        string? Role { get; }
        Guid? RoleId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        IEnumerable<string> Permissions { get; }

        Guid? UserGuid => UserId;

        bool HasPermission(string permission) => Permissions?.Contains(permission) == true;
    }
}