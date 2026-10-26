using Application.Common.Security;
using Application.Features.Users.Roles.Dtos;
using MediatR;

namespace Application.Features.Users.Roles.Queries;

[Authorize(Permissions = Permissions.Roles.View)]
public record GetRolesQuery : IRequest<Result<List<RoleDto>>>
{
    public bool IncludeUserCount { get; init; } = true;
    public string? SearchTerm { get; init; }
}