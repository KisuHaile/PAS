using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Commands;

[Authorize(Permissions = Permissions.Users.Edit)]
public record DeactivateUserCommand(Guid Id) : IRequest<Result>;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeactivateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return Result.Failure("User not found.");

        user.IsActive = false;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}
