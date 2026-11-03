using Application.Common.Interfaces;
using Application.Constants;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PAS.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _db;
    private Guid? _resolvedUserId;
    private bool _resolved;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
    }

    public Guid? UserId
    {
        get
        {
            if (_resolved)
            {
                return _resolvedUserId;
            }

            _resolved = true;

            var subject = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(subject))
            {
                _resolvedUserId = null;
                return _resolvedUserId;
            }

            // If subject is already a GUID, use it directly
            if (Guid.TryParse(subject, out var subjectGuid))
            {
                _resolvedUserId = subjectGuid;
                return _resolvedUserId;
            }

            _resolvedUserId = ResolveLegacyUserId(subject, Username);
            return _resolvedUserId;
        }
    }

    private Guid? ResolveLegacyUserId(string subject, string? username)
    {
        try
        {
            var conn = _db.Database.GetDbConnection();
            var shouldClose = conn.State != ConnectionState.Open;
            if (shouldClose)
            {
                conn.Open();
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT TOP(1) [Id]
FROM [UserLogins]
WHERE [IsDeleted] = 0
  AND [IsActive] = 1
  AND (
      CAST([Id] AS nvarchar(36)) = @subject
      OR [AspNetUserId] = @subject
      OR [Username] = @username
  )";

            var p1 = cmd.CreateParameter();
            p1.ParameterName = "@subject";
            p1.Value = subject;
            cmd.Parameters.Add(p1);

            var p2 = cmd.CreateParameter();
            p2.ParameterName = "@username";
            p2.Value = (object?)username ?? DBNull.Value;
            cmd.Parameters.Add(p2);

            var value = cmd.ExecuteScalar();

            if (shouldClose)
            {
                conn.Close();
            }

            if (value != null && Guid.TryParse(value.ToString(), out var mappedId))
            {
                return mappedId;
            }
        }
        catch
        {
            // Intentionally ignore mapping failures and treat user as unresolved.
        }

        return null;
    }

    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    public string? EmployeeCode => _httpContextAccessor.HttpContext?.User?.FindFirstValue("employeeCode");
    public string? EmployeeName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("fullName");
    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
    public Guid? RoleId => null;

    public IEnumerable<string> Permissions
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Role))
                return Enumerable.Empty<string>();

            return Role switch
            {
                SystemRoles.Admin => SystemRoles.RolePermissions.AdminPermissions,
                SystemRoles.StoreOfficer => SystemRoles.RolePermissions.StoreOfficerPermissions,
                SystemRoles.Staff => SystemRoles.RolePermissions.StaffPermissions,
                SystemRoles.Inspector => SystemRoles.RolePermissions.InspectorPermissions,
                SystemRoles.Approver => SystemRoles.RolePermissions.ApproverPermissions,
                _ => Enumerable.Empty<string>()
            };
        }
    }

    public bool IsAuthenticated => UserId.HasValue;
    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) == true;
}