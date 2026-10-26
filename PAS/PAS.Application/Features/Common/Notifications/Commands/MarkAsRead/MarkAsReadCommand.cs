using Application.Common.Security;
using MediatR;

namespace Application.Features.Common.Notifications.Commands.MarkAsRead;

[Authorize(Permissions = Permissions.Notifications.MarkAsRead)]
public record MarkAsReadCommand(Guid Id) : IRequest<Result>;