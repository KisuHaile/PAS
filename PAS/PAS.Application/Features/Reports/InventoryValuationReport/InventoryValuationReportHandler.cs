using Application.Features.Reports.InventoryValuationReport.Dtos;
using MediatR;

namespace Application.Features.Reports.InventoryValuationReport;

public class InventoryValuationReportQueryHandler : IRequestHandler<InventoryValuationReportQuery, Result<InventoryValuationReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeService _dateTime;

    public InventoryValuationReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTimeService dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<InventoryValuationReportDto>> Handle(InventoryValuationReportQuery request, CancellationToken cancellationToken)
    {
        var asOfDate = request.AsOfDate ?? _dateTime.UtcNow;

        // Get all inventory stocks with related data
        var query = _context.InventoryStocks
            .Include(i => i.Item)
                .ThenInclude(i => i.Category)
            .Include(i => i.Shelf)
                .ThenInclude(s => s.Warehouse)
            .Where(i => !i.IsDeleted && i.Item != null && !i.Item.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (request.CategoryId.HasValue)
        {
            query = query.Where(i => i.Item.CategoryId == request.CategoryId);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(i => i.Shelf.WarehouseId == request.WarehouseId);
        }

        if (!request.IncludeZeroStock)
        {
            query = query.Where(i => i.CurrentQuantity > 0);
        }

        if (!string.IsNullOrWhiteSpace(request.ItemSearch))
        {
            query = query.Where(i =>
                i.Item.ItemName.Contains(request.ItemSearch) ||
                i.Item.SKU.Contains(request.ItemSearch));
        }

        var stocks = await query.ToListAsync(cancellationToken);

        // Group by item for valuation
        var itemGroups = stocks
            .GroupBy(i => i.ItemId)
            .Select(g => new
            {
                ItemId = g.Key,
                Item = g.First().Item,
                TotalQuantity = g.Sum(i => i.CurrentQuantity),
                Locations = g.Select(i => new InventoryLocationDto
                {
                    WarehouseName = i.Shelf?.Warehouse?.WarehouseName ?? "Unknown",
                    ShelfLocation = i.Shelf != null ?
                        $"{i.Shelf.Aisle}-{i.Shelf.Rack}-{i.Shelf.ShelfNumber}" : "Unknown",
                    Quantity = i.CurrentQuantity,
                    Value = i.CurrentQuantity * GetAverageCost(i.ItemId) // You'd need to calculate this
                }).ToList()
            })
            .ToList();

        // Calculate summary
        var summary = new ReportSummary
        {
            TotalItems = itemGroups.Count,
            TotalStockItems = itemGroups.Count(g => g.TotalQuantity > 0),
            TotalQuantity = itemGroups.Sum(g => g.TotalQuantity),
            TotalValue = itemGroups.Sum(g => g.Locations.Sum(l => l.Value)),
            AverageItemValue = itemGroups.Any() ?
                itemGroups.Average(g => g.Locations.Sum(l => l.Value)) : 0,
            LowStockItems = itemGroups.Count(g => g.TotalQuantity <= (g.Item?.MinStockLevel ?? 0)),
            OutOfStockItems = itemGroups.Count(g => g.TotalQuantity <= 0)
        };

        // Create valuation items
        var valuationItems = itemGroups.Select(g => new InventoryValuationItemDto
        {
            ItemId = g.ItemId,
            SKU = g.Item?.SKU ?? string.Empty,
            ItemName = g.Item?.ItemName ?? string.Empty,
            CategoryName = g.Item?.Category?.Name ?? "Uncategorized",
            UnitOfMeasure = g.Item?.UnitOfMeasure ?? string.Empty,
            CurrentQuantity = g.TotalQuantity,
            ReservedQuantity = 0, // Would need to calculate from reservations
            AverageCost = GetAverageCost(g.ItemId),
            MinStockLevel = g.Item?.MinStockLevel ?? 0,
            Locations = g.Locations
        }).ToList();

        // Group by category
        var byCategory = valuationItems
            .GroupBy(i => i.CategoryName)
            .Select(g => new ValuationByCategoryDto
            {
                CategoryName = g.Key,
                ItemCount = g.Count(),
                TotalQuantity = g.Sum(i => i.CurrentQuantity),
                TotalValue = g.Sum(i => i.TotalValue),
                PercentageOfTotal = summary.TotalValue > 0 ?
                    (g.Sum(i => i.TotalValue) / summary.TotalValue) * 100 : 0
            })
            .OrderByDescending(c => c.TotalValue)
            .ToList();

        // Group by warehouse
        var byWarehouse = stocks
            .GroupBy(i => i.Shelf?.Warehouse?.WarehouseName ?? "Unknown")
            .Select(g => new ValuationByWarehouseDto
            {
                WarehouseName = g.Key,
                ItemCount = g.Select(i => i.ItemId).Distinct().Count(),
                TotalQuantity = g.Sum(i => i.CurrentQuantity),
                TotalValue = g.Sum(i => i.CurrentQuantity * GetAverageCost(i.ItemId)),
                PercentageOfTotal = summary.TotalValue > 0 ?
                    (g.Sum(i => i.CurrentQuantity * GetAverageCost(i.ItemId)) / summary.TotalValue) * 100 : 0
            })
            .OrderByDescending(w => w.TotalValue)
            .ToList();

        var report = new InventoryValuationReportDto
        {
            GeneratedAt = _dateTime.UtcNow,
            GeneratedBy = _currentUser.UserName ?? "System",
            Filters = new ReportFilterInfo
            {
                AsOfDate = asOfDate,
                CategoryId = request.CategoryId,
                WarehouseId = request.WarehouseId,
                IncludeZeroStock = request.IncludeZeroStock,
                ItemSearch = request.ItemSearch
            },
            Summary = summary,
            Items = valuationItems,
            ByCategory = byCategory,
            ByWarehouse = byWarehouse
        };

        return Result<InventoryValuationReportDto>.Success(report);
    }

    private decimal GetAverageCost(Guid itemId)
    {
        // This would calculate average cost from purchase history
        // Simplified for now - in reality you'd calculate from StockLedger
        return 100; // Placeholder
    }
}