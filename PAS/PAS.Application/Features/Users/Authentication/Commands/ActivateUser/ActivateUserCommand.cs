using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Commands;

[Authorize(Permissions = Permissions.Users.Edit)]
public record ActivateUserCommand(Guid Id) : IRequest<Result>;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ActivateUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return Result.Failure("User not found.");

        user.IsActive = true;
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}
