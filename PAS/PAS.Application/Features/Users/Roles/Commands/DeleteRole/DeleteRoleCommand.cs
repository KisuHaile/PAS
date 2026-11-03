using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Roles.Commands.DeleteRole;

[Authorize(Permissions = Permissions.Roles.View)]
public record DeleteRoleCommand(Guid Id) : IRequest<Result>;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteRoleCommandHandler(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());
        if (role == null) return Result.Failure("Role not found.");

        if (!string.IsNullOrWhiteSpace(role.Name) && (await _userManager.GetUsersInRoleAsync(role.Name)).Any())
            return Result.Failure("Cannot delete role assigned to users.");

        var deleteResult = await _roleManager.DeleteAsync(role);
        if (!deleteResult.Succeeded)
            return Result.Failure(string.Join("; ", deleteResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}
