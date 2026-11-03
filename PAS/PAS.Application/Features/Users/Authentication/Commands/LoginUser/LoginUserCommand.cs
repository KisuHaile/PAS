using Application.Features.Users.Authentication.Dtos;
using MediatR;

namespace Application.Features.Users.Authentication.Commands;

public record LoginUserCommand : IRequest<Result<AuthResultDto>>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool RememberMe { get; init; }
}