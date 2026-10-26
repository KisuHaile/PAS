using Application.Common.Security;
using Application.Features.Users.Roles.Dtos;
using MediatR;

namespace Application.Features.Users.Roles.Queries;

[Authorize(Permissions = Permissions.Users.View)]
public record GetUserRolesQuery(Guid UserId) : IRequest<Result<UserRoleDto>>;