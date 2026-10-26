using Application.Features.Common.Notifications.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Common.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, Result<NotificationListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetNotificationsHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<NotificationListDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<NotificationListDto>.Failure("User not authenticated.");
        }

        var query = _context.Notifications
            .Where(n => n.UserId == _currentUser.UserGuid && !n.IsDeleted)
            .AsNoTracking();

        if (request.ShowOnlyUnread == true)
        {
            query = query.Where(n => !n.IsRead);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var unreadCount = await _context.Notifications
            .CountAsync(n => n.UserId == _currentUser.UserGuid && !n.IsRead && !n.IsDeleted, cancellationToken);

        var notifications = await query
            .OrderByDescending(n => n.SentDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        // Calculate time ago for each notification
        foreach (var notification in notifications)
        {
            notification.TimeAgo = GetTimeAgo(notification.SentDate);
        }

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var result = new NotificationListDto
        {
            Notifications = notifications,
            TotalCount = totalCount,
            UnreadCount = unreadCount,
            PageNumber = request.PageNumber,
            TotalPages = totalPages
        };

        return Result<NotificationListDto>.Success(result);
    }

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return "just now";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} minute{(timeSpan.TotalMinutes >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} hour{(timeSpan.TotalHours >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} day{(timeSpan.TotalDays >= 2 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} month{(timeSpan.TotalDays / 30 >= 2 ? "s" : "")} ago";

        return $"{(int)(timeSpan.TotalDays / 365)} year{(timeSpan.TotalDays / 365 >= 2 ? "s" : "")} ago";
    }
}