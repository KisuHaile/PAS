using Application.Features.Reports.StockMovementReport.Dtos;
using MediatR;

namespace Application.Features.Reports.StockMovementReport;

public class StockMovementReportQueryHandler : IRequestHandler<StockMovementReportQuery, Result<StockMovementReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public StockMovementReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<StockMovementReportDto>> Handle(StockMovementReportQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockLedgers
            .Include(l => l.Item)
            .Include(l => l.Shelf)
                .ThenInclude(s => s.Warehouse)
            .Where(l => !l.IsDeleted && l.CreatedDate >= request.FromDate && l.CreatedDate <= request.ToDate)
            .AsNoTracking();

        // Apply filters
        if (request.ItemId.HasValue)
        {
            query = query.Where(l => l.ItemId == request.ItemId);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(l => l.Shelf.WarehouseId == request.WarehouseId);
        }

        if (!string.IsNullOrWhiteSpace(request.TransactionType))
        {
            query = query.Where(l => l.TransactionType == request.TransactionType);
        }

        var movements = await query.ToListAsync(cancellationToken);

        // Calculate summary
        var summary = new MovementSummary
        {
            TotalTransactions = movements.Count,
            TotalInbound = movements.Count(m => m.QuantityChange > 0),
            TotalOutbound = movements.Count(m => m.QuantityChange < 0),
            TotalAdjustments = movements.Count(m => m.TransactionType == "ADJUSTMENT"),
            TotalQuantityIn = movements.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
            TotalQuantityOut = Math.Abs(movements.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
            UniqueItemsMoved = movements.Select(m => m.ItemId).Distinct().Count()
        };

        // Group by transaction type
        var byType = movements
            .GroupBy(m => m.TransactionType)
            .Select(g => new MovementByTypeDto
            {
                TransactionType = g.Key,
                Count = g.Count(),
                Quantity = Math.Abs(g.Sum(m => m.QuantityChange)),
                Percentage = movements.Count > 0 ? (double)g.Count() / movements.Count * 100 : 0
            })
            .OrderByDescending(t => t.Count)
            .ToList();

        // Get top moving items
        var topMovingItems = movements
            .GroupBy(m => new { m.ItemId, m.Item })
            .Select(g => new MovementByItemDto
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.Item?.ItemName ?? "Unknown",
                SKU = g.Key.Item?.SKU ?? "Unknown",
                TotalQuantity = Math.Abs(g.Sum(m => m.QuantityChange)),
                TransactionCount = g.Count(),
                InboundQuantity = g.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
                OutboundQuantity = Math.Abs(g.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
                NetMovement = g.Sum(m => m.QuantityChange)
            })
            .OrderByDescending(i => i.TotalQuantity)
            .Take(20)
            .ToList();

        // Daily trend
        var dailyTrend = movements
            .GroupBy(m => m.CreatedDate.Date)
            .Select(g => new MovementTrendDto
            {
                Date = g.Key,
                Inbound = g.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
                Outbound = Math.Abs(g.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
                Net = g.Sum(m => m.QuantityChange)
            })
            .OrderBy(t => t.Date)
            .ToList();

        // Detail movements
        var details = movements.Select(m => new StockMovementDetailDto
        {
            Date = m.CreatedDate,
            TransactionType = m.TransactionType,
            ItemName = m.Item?.ItemName ?? "Unknown",
            SKU = m.Item?.SKU ?? "Unknown",
            QuantityChange = m.QuantityChange,
            Warehouse = m.Shelf?.Warehouse?.WarehouseName ?? "Unknown",
            ShelfLocation = m.Shelf != null ?
                $"{m.Shelf.Aisle}-{m.Shelf.Rack}-{m.Shelf.ShelfNumber}" : "Unknown",
            Reference = m.ReferenceId.ToString(),
            PerformedBy = "System" // Would need to track user
        }).ToList();

        var report = new StockMovementReportDto
        {
            GeneratedAt = _dateTime.UtcNow,
            GeneratedBy = _currentUser.UserName ?? "System",
            Period = new ReportPeriod
            {
                FromDate = request.FromDate,
                ToDate = request.ToDate
            },
            Summary = summary,
            ByType = byType,
            TopMovingItems = topMovingItems,
            DailyTrend = dailyTrend,
            Movements = details
        };

        return Result<StockMovementReportDto>.Success(report);
    }
}