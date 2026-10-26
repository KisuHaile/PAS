using Application.Features.Users.Roles.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Roles.Queries;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<UserRoleDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetUserRolesQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<UserRoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), request.UserId);
        }

        var allRoles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        var assignedRoles = await _userManager.GetRolesAsync(user);

        var result = new UserRoleDto
        {
            UserId = request.UserId,
            Username = user.UserName ?? string.Empty,
            EmployeeName = user.FullName,
            Roles = allRoles.Select(r => new RoleAssignmentDto
            {
                RoleId = Guid.TryParse(r.Id, out var roleId) ? roleId : Guid.Empty,
                RoleName = r.Name ?? string.Empty,
                IsAssigned = r.Name != null && assignedRoles.Contains(r.Name)
            }).ToList()
        };

        return Result<UserRoleDto>.Success(result);
    }
}