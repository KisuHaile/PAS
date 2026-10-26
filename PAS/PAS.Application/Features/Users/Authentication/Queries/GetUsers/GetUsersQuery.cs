using Application.Common.Security;
using Application.Features.Users.Authentication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Queries;

[Authorize(Permissions = Permissions.Users.View)]
public record GetUsersQuery : IRequest<Result<PaginatedList<UserDto>>>
{
    public string? SearchTerm { get; init; }
    public Guid? RoleId { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetUsersQueryHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(u =>
                (u.UserName != null && u.UserName.Contains(request.SearchTerm)) ||
                (u.Email != null && u.Email.Contains(request.SearchTerm)));

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        if (request.RoleId.HasValue)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId.Value.ToString());
            if (role == null || string.IsNullOrWhiteSpace(role.Name))
                return Result<PaginatedList<UserDto>>.Success(new PaginatedList<UserDto>(new List<UserDto>(), 0, request.PageNumber, request.PageSize));

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            var roleUserIds = usersInRole.Select(u => u.Id).ToHashSet();
            query = query.Where(u => roleUserIds.Contains(u.Id));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query.OrderBy(u => u.UserName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserDto>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var primaryRoleName = roles.FirstOrDefault() ?? string.Empty;
            var roleEntity = string.IsNullOrWhiteSpace(primaryRoleName)
                ? null
                : await _roleManager.FindByNameAsync(primaryRoleName);

            items.Add(new UserDto
            {
                Id = Guid.TryParse(user.Id, out var userId) ? userId : Guid.Empty,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                RoleId = roleEntity != null && Guid.TryParse(roleEntity.Id, out var roleId) ? roleId : Guid.Empty,
                RoleName = primaryRoleName,
                ProfileImageUrl = user.ProfileImageUrl
            });
        }

        var page = new PaginatedList<UserDto>(items, totalCount, request.PageNumber, request.PageSize);
        return Result<PaginatedList<UserDto>>.Success(page);
    }
}
