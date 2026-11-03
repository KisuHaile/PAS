using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Roles.Commands.CreateRole;

[Authorize(Permissions = Permissions.Roles.View)]
public record CreateRoleCommand : IRequest<Result<Guid>>
{
    public string RoleName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _roleManager.RoleExistsAsync(request.RoleName))
            return Result<Guid>.Failure("Role already exists.");

        var role = new ApplicationRole { Name = request.RoleName, Description = request.Description };
        var createResult = await _roleManager.CreateAsync(role);
        if (!createResult.Succeeded)
            return Result<Guid>.Failure(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        return Result<Guid>.Success(Guid.TryParse(role.Id, out var roleId) ? roleId : Guid.Empty);
    }
}
