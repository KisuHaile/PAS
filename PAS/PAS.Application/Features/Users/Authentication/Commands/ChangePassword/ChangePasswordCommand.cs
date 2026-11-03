using MediatR;

namespace Application.Features.Users.Authentication.Commands;

public record ChangePasswordCommand : IRequest<Result>
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
