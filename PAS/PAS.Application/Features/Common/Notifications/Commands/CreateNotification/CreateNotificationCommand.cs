using MediatR;

namespace Application.Features.Common.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; init; }
    public string Message { get; init; } = string.Empty;
}