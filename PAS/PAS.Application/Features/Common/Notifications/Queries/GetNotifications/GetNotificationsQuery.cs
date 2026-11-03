using Application.Common.Security;
using Application.Features.Common.Notifications.Dtos;
using MediatR;

namespace Application.Features.Common.Notifications.Queries.GetNotifications;

[Authorize(Permissions = Permissions.Notifications.View)]
public record GetNotificationsQuery : IRequest<Result<NotificationListDto>>
{
    public bool? ShowOnlyUnread { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}