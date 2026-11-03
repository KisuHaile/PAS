using Application.Features.Storage.Warehouses.Dtos;
using AutoMapper;
using MediatR;
using Domain.Storage;

namespace Application.Features.Storage.Warehouses.Queries;

public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, Result<WarehouseDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetWarehouseByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<WarehouseDetailDto>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses
            .Include(w => w.ShelfLocations)
                .ThenInclude(s => s.InventoryStocks)
                    .ThenInclude(i => i.Item)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.Warehouse), request.Id);
        }

        var warehouseDto = _mapper.Map<WarehouseDetailDto>(warehouse);

        // Calculate shelf statistics
        var totalShelves = warehouse.ShelfLocations?.Count(s => !s.IsDeleted) ?? 0;
        var occupiedShelves = warehouse.ShelfLocations?
            .Count(s => !s.IsDeleted && s.InventoryStocks != null && s.InventoryStocks.Any(i => i.CurrentQuantity > 0)) ?? 0;

        warehouseDto.TotalShelves = totalShelves;
        warehouseDto.OccupiedShelves = occupiedShelves;

        // Map shelves
        warehouseDto.Shelves = warehouse.ShelfLocations?
            .Where(s => !s.IsDeleted)
            .Select(s => new WarehouseShelfDto
            {
                Id = s.Id,
                Aisle = s.Aisle,
                Rack = s.Rack,
                ShelfNumber = s.ShelfNumber,
                FullLocation = $"{s.Aisle}-{s.Rack}-{s.ShelfNumber}",
                ItemCount = s.InventoryStocks?.Count(i => i.CurrentQuantity > 0) ?? 0,
                TotalQuantity = s.InventoryStocks?.Sum(i => i.CurrentQuantity) ?? 0,
                UtilizationPercentage = s.InventoryStocks != null && s.InventoryStocks.Any() ?
                    (double)s.InventoryStocks.Count(i => i.CurrentQuantity > 0) /
                    (s.InventoryStocks.Count > 0 ? s.InventoryStocks.Count : 1) * 100 : 0
            })
            .OrderBy(s => s.Aisle)
            .ThenBy(s => s.Rack)
            .ThenBy(s => s.ShelfNumber)
            .ToList() ?? new();

        // Get top items by quantity
        var topItems = warehouse.ShelfLocations?
            .SelectMany(s => s.InventoryStocks)
            .Where(i => i.CurrentQuantity > 0)
            .GroupBy(i => new { i.ItemId, i.Item })
            .Select(g => new WarehouseInventorySummaryDto
            {
                ItemId = g.Key.ItemId,
                ItemName = g.Key.Item?.ItemName ?? "Unknown",
                SKU = g.Key.Item?.SKU ?? "Unknown",
                Quantity = g.Sum(i => i.CurrentQuantity),
                ReservedQuantity = g.Sum(i => i.ReservedQuantity)
            })
            .OrderByDescending(i => i.Quantity)
            .Take(10)
            .ToList() ?? new();

        warehouseDto.TopItems = topItems;
        warehouseDto.TotalItems = topItems.Count;

        return Result<WarehouseDetailDto>.Success(warehouseDto);
    }
}