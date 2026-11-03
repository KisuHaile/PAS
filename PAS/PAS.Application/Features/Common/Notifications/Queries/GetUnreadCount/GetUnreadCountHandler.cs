using MediatR;

namespace Application.Features.Common.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<int>.Success(0);
        }

        var count = await _context.Notifications
            .CountAsync(n => n.UserId == _currentUser.UserGuid && !n.IsRead && !n.IsDeleted, cancellationToken);

        return Result<int>.Success(count);
    }
}