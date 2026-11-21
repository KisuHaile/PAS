using Application.Features.Common.AuditTrail.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;

namespace Application.Features.Common.AuditTrail.Queries.GetAuditSummary;

public class GetAuditSummaryHandler : IRequestHandler<GetAuditSummaryQuery, Result<AuditSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GetAuditSummaryHandler(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<Result<AuditSummaryDto>> Handle(GetAuditSummaryQuery request, CancellationToken cancellationToken)
    {
        // East Africa Time (EAT) has a constant fixed offset of UTC+3 (no daylight saving time).
        var eatOffset = TimeSpan.FromHours(3);
        var eatNow = DateTimeOffset.UtcNow.ToOffset(eatOffset);

        // Start of today in EAT (00:00:00) converted to UTC
        var eatTodayStart = new DateTimeOffset(eatNow.Year, eatNow.Month, eatNow.Day, 0, 0, 0, eatOffset);
        var utcTodayStart = eatTodayStart.UtcDateTime;

        // Start of this week in EAT (Monday 00:00:00) converted to UTC
        // DayOfWeek: Sunday = 0, Monday = 1, Tuesday = 2, ..., Saturday = 6
        int daysSinceMonday = ((int)eatNow.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var eatWeekStart = eatTodayStart.AddDays(-daysSinceMonday);
        var utcWeekStart = eatWeekStart.UtcDateTime;

        var baseQuery = _context.AuditTrails
            .Where(a => !a.IsDeleted)
            .AsNoTracking();

        var totalEvents = await baseQuery.CountAsync(cancellationToken);

        var todayEvents = await baseQuery
            .Where(a => a.ActionDate >= utcTodayStart)
            .CountAsync(cancellationToken);

        var thisWeekEvents = await baseQuery
            .Where(a => a.ActionDate >= utcWeekStart)
            .CountAsync(cancellationToken);

        // "Most active user" must ignore rows where UserId is Guid.Empty (the "System" rows).
        var topUserGroup = await baseQuery
            .Where(a => a.UserId != Guid.Empty)
            .GroupBy(a => a.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .FirstOrDefaultAsync(cancellationToken);

        MostActiveUserDto? mostActiveUser = null;
        if (topUserGroup != null)
        {
            var user = await _userManager.FindByIdAsync(topUserGroup.UserId.ToString());
            var userName = user != null
                ? (!string.IsNullOrWhiteSpace(user.UserName) ? user.UserName : (user.FullName ?? "Unknown"))
                : "Unknown";

            mostActiveUser = new MostActiveUserDto
            {
                UserId = topUserGroup.UserId,
                UserName = userName,
                ActionCount = topUserGroup.Count
            };
        }

        var summary = new AuditSummaryDto
        {
            TotalEvents = totalEvents,
            TodayEvents = todayEvents,
            ThisWeekEvents = thisWeekEvents,
            MostActiveUser = mostActiveUser,
            SuspiciousCount = null,
            SuspiciousActivityNote = "Suspicious activity detection is not implemented yet."
        };

        return Result<AuditSummaryDto>.Success(summary);
    }
}
