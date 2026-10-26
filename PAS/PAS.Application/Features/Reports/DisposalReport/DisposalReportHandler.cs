using Application.Features.Reports.DisposalReport.Dtos;
using MediatR;

namespace Application.Features.Reports.DisposalReport;

public class DisposalReportQueryHandler : IRequestHandler<DisposalReportQuery, Result<DisposalReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public DisposalReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<DisposalReportDto>> Handle(DisposalReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DisposalRecords
            .Include(d => d.Item)
            .Where(d => !d.IsDeleted && d.DisposalDate >= request.FromDate && d.DisposalDate <= request.ToDate)
            .AsNoTracking();

        // Apply additional filters
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(d => d.Status == request.Status);
        }

        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            query = query.Where(d => d.Reason.Contains(request.Reason));
        }

        if (!string.IsNullOrWhiteSpace(request.Method))
        {
            query = query.Where(d => d.DisposalMethod == request.Method);
        }

        if (request.DisposedBy.HasValue)
        {
            query = query.Where(d => d.DisposedById == request.DisposedBy);
        }

        var disposals = await query.ToListAsync(cancellationToken);

        // Calculate summary
        var summary = new DisposalSummary
        {
            TotalDisposals = disposals.Count,
            TotalItems = disposals.Select(d => d.ItemId).Distinct().Count(),
            TotalQuantity = disposals.Sum(d => d.Quantity),
            TotalEstimatedValue = disposals.Sum(d => d.EstimatedValue ?? 0),
            AverageValuePerItem = disposals.Any() ?
                disposals.Average(d => d.EstimatedValue ?? 0) : 0,
            PendingApprovals = disposals.Count(d => d.Status == "PENDING_APPROVAL"),
            ApprovedDisposals = disposals.Count(d => d.Status == "APPROVED"),
            RejectedDisposals = disposals.Count(d => d.Status == "REJECTED")
        };

        // Group by reason
        var byReason = disposals
            .GroupBy(d => d.Reason)
            .Select(g => new DisposalByReasonDto
            {
                Reason = g.Key,
                Count = g.Count(),
                Quantity = g.Sum(d => d.Quantity),
                TotalValue = g.Sum(d => d.EstimatedValue ?? 0),
                Percentage = disposals.Count > 0 ?
                    (double)g.Count() / disposals.Count * 100 : 0
            })
            .OrderByDescending(r => r.Count)
            .ToList();

        // Group by method
        var byMethod = disposals
            .GroupBy(d => d.DisposalMethod ?? "Unknown")
            .Select(g => new DisposalByMethodDto
            {
                Method = g.Key,
                Count = g.Count(),
                Quantity = g.Sum(d => d.Quantity),
                TotalValue = g.Sum(d => d.EstimatedValue ?? 0),
                Percentage = disposals.Count > 0 ?
                    (double)g.Count() / disposals.Count * 100 : 0
            })
            .OrderByDescending(m => m.Count)
            .ToList();

        // Group by month
        var byMonth = disposals
            .GroupBy(d => new { d.DisposalDate.Year, d.DisposalDate.Month })
            .Select(g => new DisposalByMonthDto
            {
                Year = g.Key.Year,
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Count = g.Count(),
                Quantity = g.Sum(d => d.Quantity),
                Value = g.Sum(d => d.EstimatedValue ?? 0)
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        // Create detail list
        var details = disposals.Select(d => new DisposalDetailDto
        {
            Id = d.Id,
            DisposalDate = d.DisposalDate,
            ItemName = d.Item?.ItemName ?? "Unknown",
            ItemSKU = d.Item?.SKU ?? "Unknown",
            Quantity = d.Quantity,
            Reason = d.Reason,
            Method = d.DisposalMethod ?? "Unknown",
            Status = d.Status,
            EstimatedValue = d.EstimatedValue ?? 0,
            DisposedBy = string.Empty,
            ApprovedBy = string.Empty,
            ApprovedDate = d.ApprovedDate
        }).ToList();

        var report = new DisposalReportDto
        {
            GeneratedAt = _dateTime.UtcNow,
            GeneratedBy = _currentUser.UserName ?? "System",
            Period = new ReportPeriod
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate
            },
            Summary = summary,
            ByReason = byReason,
            ByMethod = byMethod,
            ByMonth = byMonth,
            Disposals = details
        };

        return Result<DisposalReportDto>.Success(report);
    }
}