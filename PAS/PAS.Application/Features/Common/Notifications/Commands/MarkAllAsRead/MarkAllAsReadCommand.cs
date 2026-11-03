using Application.Common.Security;
using MediatR;

namespace Application.Features.Common.Notifications.Commands.MarkAllAsRead;

[Authorize(Permissions = Permissions.Notifications.MarkAsRead)]
public record MarkAllAsReadCommand : IRequest<Result>;