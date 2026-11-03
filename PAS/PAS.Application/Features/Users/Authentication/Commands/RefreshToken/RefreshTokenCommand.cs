using Application.Features.Users.Authentication.Dtos;
using MediatR;

namespace Application.Features.Users.Authentication.Commands;

public record RefreshTokenCommand : IRequest<Result<AuthResultDto>>
{
    public string RefreshToken { get; init; } = string.Empty;
}
