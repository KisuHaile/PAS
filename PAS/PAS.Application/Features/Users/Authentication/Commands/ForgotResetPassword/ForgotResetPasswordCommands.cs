using MediatR;

namespace Application.Features.Users.Authentication.Commands;

public record ForgotPasswordCommand : IRequest<Result<ForgotPasswordResponseDto>>
{
    public string Email { get; init; } = string.Empty;
}

public record ForgotPasswordResponseDto
{
    public bool EmailSent { get; init; }
    public string Token { get; init; } = string.Empty;
    public string EncodedToken { get; init; } = string.Empty;
};

public record ResetPasswordCommand : IRequest<Result>
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
