using Application.Features.Users.Roles.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Roles.Queries;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleDto>>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetRolesQueryHandler(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Result<List<RoleDto>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var rolesQuery = _roleManager.Roles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            rolesQuery = rolesQuery.Where(r => r.Name != null && r.Name.Contains(request.SearchTerm));
        }

        var roles = await rolesQuery
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        var roleDtos = new List<RoleDto>(roles.Count);

        foreach (var role in roles)
        {
            var roleUsers = role.Name is null
                ? new List<ApplicationUser>()
                : (await _userManager.GetUsersInRoleAsync(role.Name)).ToList();

            roleDtos.Add(new RoleDto
            {
                Id = Guid.TryParse(role.Id, out var parsedRoleId) ? parsedRoleId : Guid.Empty,
                RoleName = role.Name ?? string.Empty,
                Description = role.Description,
                UserCount = request.IncludeUserCount ? roleUsers.Count : 0,
                Permissions = GetPermissionsForRole(role.Name ?? string.Empty)
            });
        }

        return Result<List<RoleDto>>.Success(roleDtos);
    }

    private List<string> GetPermissionsForRole(string roleName)
    {
        return roleName switch
        {
            SystemRoles.Admin => SystemRoles.RolePermissions.AdminPermissions.ToList(),
            SystemRoles.StoreOfficer => SystemRoles.RolePermissions.StoreOfficerPermissions.ToList(),
            SystemRoles.Staff => SystemRoles.RolePermissions.StaffPermissions.ToList(),
            SystemRoles.Inspector => SystemRoles.RolePermissions.InspectorPermissions.ToList(),
            SystemRoles.Approver => SystemRoles.RolePermissions.ApproverPermissions.ToList(),
            SystemRoles.Manager => SystemRoles.RolePermissions.ManagerPermissions.ToList(),
            _ => new List<string>()
        };
    }
}