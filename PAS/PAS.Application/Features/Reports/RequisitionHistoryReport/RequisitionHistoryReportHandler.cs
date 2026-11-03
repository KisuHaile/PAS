using Application.Features.Reports.RequisitionHistoryReport.Dtos;
using MediatR;

namespace Application.Features.Reports.RequisitionHistoryReport;

public class RequisitionHistoryReportQueryHandler : IRequestHandler<RequisitionHistoryReportQuery, Result<RequisitionHistoryReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public RequisitionHistoryReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<RequisitionHistoryReportDto>> Handle(RequisitionHistoryReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ServiceRequests
            .Include(s => s.Details)
                .ThenInclude(d => d.Item)
            .Include(s => s.StoreIssueVoucher)
            .Where(s => !s.IsDeleted && s.RequestDate >= request.FromDate && s.RequestDate <= request.ToDate)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(s => s.Status == request.Status);
        }

        if (!string.IsNullOrWhiteSpace(request.Department))
        {
            query = query.Where(s => false);
        }

        if (request.RequesterId.HasValue)
        {
            query = query.Where(s => s.RequesterId == request.RequesterId);
        }

        var requisitions = await query.ToListAsync(cancellationToken);

        // Calculate summary
        var summary = new RequisitionSummary
        {
            TotalRequisitions = requisitions.Count,
            TotalItems = requisitions.Sum(r => r.Details?.Count ?? 0),
            TotalQuantity = requisitions.Sum(r => r.Details?.Sum(d => d.RequestedQty) ?? 0),
            IssuedQuantity = requisitions.Sum(r => r.Details?.Sum(d => d.IssuedQty) ?? 0),
            TotalValue = 0, // Would need item prices
            PendingCount = requisitions.Count(r => r.Status == "Pending"),
            ApprovedCount = requisitions.Count(r => r.Status == "Approved"),
            RejectedCount = requisitions.Count(r => r.Status == "Rejected"),
            IssuedCount = requisitions.Count(r => r.Status == "Issued"),
            CompletedCount = requisitions.Count(r => r.Status == "Completed")
        };

        // Group by status
        var byStatus = requisitions
            .GroupBy(r => r.Status)
            .Select(g => new RequisitionByStatusDto
            {
                Status = g.Key,
                Count = g.Count(),
                Items = g.Sum(r => r.Details?.Count ?? 0),
                Quantity = g.Sum(r => r.Details?.Sum(d => d.RequestedQty) ?? 0),
                Percentage = requisitions.Count > 0 ? (double)g.Count() / requisitions.Count * 100 : 0
            })
            .OrderByDescending(s => s.Count)
            .ToList();

        // Group by department
        var byDepartment = requisitions
            .GroupBy(r => string.Empty)
            .Select(g => new RequisitionByDepartmentDto
            {
                Department = g.Key,
                Count = g.Count(),
                Requestors = g.Select(r => r.RequesterId).Distinct().Count(),
                Quantity = g.Sum(r => r.Details?.Sum(d => d.RequestedQty) ?? 0),
                Percentage = requisitions.Count > 0 ? (double)g.Count() / requisitions.Count * 100 : 0
            })
            .OrderByDescending(d => d.Count)
            .ToList();

        // Group by month
        var byMonth = requisitions
            .GroupBy(r => new { r.RequestDate.Year, r.RequestDate.Month })
            .Select(g => new RequisitionByMonthDto
            {
                Year = g.Key.Year,
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                Count = g.Count(),
                Quantity = g.Sum(r => r.Details?.Sum(d => d.RequestedQty) ?? 0),
                IssuedQuantity = g.Sum(r => r.Details?.Sum(d => d.IssuedQty) ?? 0)
            })
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();

        // Create detail list
        var details = requisitions.Select(r => new RequisitionDetailDto
        {
            Id = r.Id,
            SRNumber = r.SRNumber,
            RequestDate = r.RequestDate,
            RequesterName = string.Empty,
            Department = string.Empty,
            Status = r.Status,
            ItemCount = r.Details?.Count ?? 0,
            TotalQuantity = r.Details?.Sum(d => d.RequestedQty) ?? 0,
            IssuedQuantity = r.Details?.Sum(d => d.IssuedQty) ?? 0,
            ApproverName = string.Empty,
            ApprovedDate = null, // Would need to track approval date
            SIVNumber = r.StoreIssueVoucher?.SIVNumber,
            IssueDate = r.StoreIssueVoucher?.IssueDate,
            Items = r.Details?.Select(d => new RequisitionItemDto
            {
                ItemName = d.Item?.ItemName ?? "Unknown",
                SKU = d.Item?.SKU ?? "Unknown",
                RequestedQty = d.RequestedQty,
                IssuedQty = d.IssuedQty,
                UnitOfMeasure = d.Item?.UnitOfMeasure ?? string.Empty
            }).ToList() ?? new()
        }).ToList();

        var report = new RequisitionHistoryReportDto
        {
            GeneratedAt = _dateTime.UtcNow,
            GeneratedBy = _currentUser.UserName ?? "System",
            Period = new ReportPeriod
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate
            },
            Summary = summary,
            ByStatus = byStatus,
            ByDepartment = byDepartment,
            ByMonth = byMonth,
            Requisitions = details
        };

        return Result<RequisitionHistoryReportDto>.Success(report);
    }
}