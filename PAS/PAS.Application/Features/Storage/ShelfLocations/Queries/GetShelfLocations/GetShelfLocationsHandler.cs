using Application.Features.Storage.ShelfLocations.Dtos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Storage.ShelfLocations.Queries;

public class GetShelfLocationsQueryHandler : IRequestHandler<GetShelfLocationsQuery, Result<PaginatedList<ShelfLocationListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetShelfLocationsQueryHandler> _logger;

    public GetShelfLocationsQueryHandler(IApplicationDbContext context, IMapper mapper, ILogger<GetShelfLocationsQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<PaginatedList<ShelfLocationListDto>>> Handle(GetShelfLocationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _context.ShelfLocations
                .Include(s => s.Warehouse)
                .Include(s => s.InventoryStocks)
                .Where(s => !s.IsDeleted)
                .AsNoTracking();

            // Apply filters
            if (request.WarehouseId.HasValue)
            {
                query = query.Where(s => s.WarehouseId == request.WarehouseId);
            }

            if (!string.IsNullOrWhiteSpace(request.Zone))
            {
                query = query.Where(s => s.Zone == request.Zone);
            }

            if (!string.IsNullOrWhiteSpace(request.BinType))
            {
                query = query.Where(s => s.BinType == request.BinType);
            }


            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive);
            }

        if (request.HasInventory.HasValue)
        {
            if (request.HasInventory.Value)
                    query = query.Where(s => s.InventoryStocks.Any(i => i.CurrentQuantity > 0));
            else
                    query = query.Where(s => !s.InventoryStocks.Any(i => i.CurrentQuantity > 0));
        }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(s =>
                    s.Aisle.Contains(request.SearchTerm) ||
                    s.Rack.Contains(request.SearchTerm) ||
                    s.ShelfNumber.Contains(request.SearchTerm) ||
                    s.QRCodeValue.Contains(request.SearchTerm) ||
                    (s.Zone != null && s.Zone.Contains(request.SearchTerm)));
            }

            var projectedQuery = query.Select(s => new ShelfLocationListDto
            {
                Id = s.Id,
                WarehouseName = s.Warehouse != null ? s.Warehouse.WarehouseName : string.Empty,
                FullAddress = $"{s.Aisle}-{s.Rack}-{s.ShelfNumber}",
                QRCodeValue = s.QRCodeValue,
                IsActive = s.IsActive,
                ItemCount = s.InventoryStocks.Count(i => i.CurrentQuantity > 0),
                TotalQuantity = s.InventoryStocks.Sum(i => i.CurrentQuantity),
                Capacity = s.Capacity
            });

            var paginatedShelves = await projectedQuery
                .OrderBy(s => s.WarehouseName)
                .ThenBy(s => s.FullAddress)
                .PaginatedListAsync(request.PageNumber, request.PageSize);

            return Result<PaginatedList<ShelfLocationListDto>>.Success(paginatedShelves);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shelf locations");
            return Result<PaginatedList<ShelfLocationListDto>>.Failure("An error occurred while retrieving shelf locations.");
        }
    }
}
