using Application.Common.Security;
using Application.Features.Users.Authentication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Queries;

[Authorize(Permissions = Permissions.Users.View)]
public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDetailDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());

        if (user == null)
            return Result<UserDetailDto>.Failure("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        var primaryRoleName = roles.FirstOrDefault() ?? string.Empty;
        var roleEntity = string.IsNullOrWhiteSpace(primaryRoleName)
            ? null
            : await _roleManager.FindByNameAsync(primaryRoleName);

        var dto = new UserDetailDto
        {
            Id = request.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            RoleId = roleEntity != null && Guid.TryParse(roleEntity.Id, out var roleId) ? roleId : Guid.Empty,
            RoleName = primaryRoleName,
            ProfileImageUrl = user.ProfileImageUrl,
            EmployeeId = Guid.Empty,
            EmployeeCode = string.Empty,
            EmployeeName = string.Empty
        };

        return Result<UserDetailDto>.Success(dto);
    }
}
