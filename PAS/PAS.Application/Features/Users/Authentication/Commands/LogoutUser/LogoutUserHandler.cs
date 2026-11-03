using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Authentication.Commands;

public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<LogoutUserCommandHandler> _logger;

    public LogoutUserCommandHandler(
        ICurrentUserService currentUser,
        ILogger<LogoutUserCommandHandler> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public Task<Result> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User logged out: {UserId}", _currentUser.UserId);

        // In a real application, you might want to invalidate the token
        // by adding it to a blacklist or clearing server-side sessions

        return Task.FromResult(Result.Success());
    }
}