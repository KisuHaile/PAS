using Application.Features.Storage.ShelfLocations.Dtos;
using MediatR;

namespace Application.Features.Storage.ShelfLocations.Queries;

public class GetShelfLocationByIdQueryHandler : IRequestHandler<GetShelfLocationByIdQuery, Result<ShelfLocationDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetShelfLocationByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ShelfLocationDetailDto>> Handle(GetShelfLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var shelfLocation = await _context.ShelfLocations
            .Include(s => s.Warehouse)
            .Include(s => s.InventoryStocks)
                .ThenInclude(i => i.Item)
            .Include(s => s.InventoryStocks)
                .ThenInclude(i => i.StockLedgerEntries)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (shelfLocation == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.ShelfLocation), request.Id);
        }

        var shelfDto = _mapper.Map<ShelfLocationDetailDto>(shelfLocation);

        // Calculate current stats
        shelfDto.CurrentItemCount = shelfLocation.InventoryStocks?.Count(i => i.CurrentQuantity > 0) ?? 0;
        shelfDto.CurrentQuantity = shelfLocation.InventoryStocks?.Sum(i => i.CurrentQuantity) ?? 0;
        shelfDto.UtilizationPercentage = shelfLocation.Capacity > 0 ?
            (double)shelfDto.CurrentQuantity / shelfLocation.Capacity * 100 : 0;

        // Map inventory items
        shelfDto.Inventory = shelfLocation.InventoryStocks?
            .Where(i => i.CurrentQuantity > 0)
            .Select(i => new ShelfInventoryItemDto
            {
                InventoryId = i.Id,
                ItemId = i.ItemId,
                ItemName = i.Item?.ItemName ?? "Unknown",
                SKU = i.Item?.SKU ?? "Unknown",
                CurrentQuantity = i.CurrentQuantity,
                ReservedQuantity = i.ReservedQuantity,
                UnitOfMeasure = i.Item?.UnitOfMeasure ?? string.Empty,
                BatchNumber = i.BatchNumber,
                ExpiryDate = i.ExpiryDate,
                LastUpdated = i.UpdatedAt ?? i.CreatedAt
            })
            .OrderBy(i => i.ItemName)
            .ToList() ?? new();

        // Get recent movements
        shelfDto.RecentMovements = shelfLocation.InventoryStocks?
            .SelectMany(i => i.StockLedgerEntries)
            .Where(l => !l.IsDeleted)
            .OrderByDescending(l => l.CreatedDate)
            .Take(20)
            .Select(l => new ShelfMovementDto
            {
                Date = l.CreatedDate,
                TransactionType = l.TransactionType,
                ItemName = l.Item?.ItemName ?? "Unknown",
                QuantityChange = l.QuantityChange,
                Reference = l.ReferenceId.ToString()
            })
            .ToList() ?? new();

        return Result<ShelfLocationDetailDto>.Success(shelfDto);
    }
}
