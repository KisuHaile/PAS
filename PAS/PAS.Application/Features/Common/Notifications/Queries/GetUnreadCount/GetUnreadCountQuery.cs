using Application.Common.Security;
using MediatR;

namespace Application.Features.Common.Notifications.Queries.GetUnreadCount;

[Authorize(Permissions = Permissions.Notifications.View)]
public record GetUnreadCountQuery : IRequest<Result<int>>;