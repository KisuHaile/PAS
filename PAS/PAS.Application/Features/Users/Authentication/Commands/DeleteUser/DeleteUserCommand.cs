using Application.Common.Security;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Users.Authentication.Commands;

[Authorize(Permissions = Permissions.Users.Delete)]
public record DeleteUserCommand(Guid Id) : IRequest<Result>;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null) return Result.Failure("User not found.");

        var deleteResult = await _userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
            return Result.Failure(string.Join("; ", deleteResult.Errors.Select(e => e.Description)));

        return Result.Success();
    }
}
