using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Roles.Commands.UpdateRole;

[Authorize(Permissions = Permissions.Roles.View)]
public record UpdateRoleCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UpdateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null) return Result.Failure("Role not found.");

        role.Name = request.RoleName;
        role.Description = request.Description;
        var updateResult = await _roleManager.UpdateAsync(role);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}
