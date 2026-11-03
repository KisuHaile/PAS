using MediatR;

namespace Application.Features.Users.Authentication.Commands;

public record LogoutUserCommand : IRequest<Result>;