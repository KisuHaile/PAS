using Application.Features.Storage.InventoryStock.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Queries;

public class GetInventoryStockQueryHandler : IRequestHandler<GetInventoryStockQuery, Result<PaginatedList<InventoryStockDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTimeService _dateTime;

    public GetInventoryStockQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IDateTimeService dateTime)
    {
        _context = context;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<Result<PaginatedList<InventoryStockDto>>> Handle(GetInventoryStockQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryStocks
            .Include(i => i.Item)
            .Include(i => i.ShelfLocation)
                .ThenInclude(s => s.Warehouse)
            .Where(i => !i.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (request.ItemId.HasValue)
        {
            query = query.Where(i => i.ItemId == request.ItemId);
        }

        if (request.ShelfId.HasValue)
        {
            query = query.Where(i => i.ShelfId == request.ShelfId);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(i => i.ShelfLocation != null && i.ShelfLocation.WarehouseId == request.WarehouseId);
        }

        if (request.LowStockOnly == true)
        {
            query = query.Where(i => i.CurrentQuantity <= i.Item.MinStockLevel);
        }

        if (request.ExpiringSoon == true)
        {
            var expiryThreshold = _dateTime.UtcNow.AddDays(request.ExpiryDays ?? 30);
            query = query.Where(i => i.ExpiryDate != null && i.ExpiryDate <= expiryThreshold);
        }

        if (!string.IsNullOrWhiteSpace(request.BatchNumber))
        {
            query = query.Where(i => i.BatchNumber == request.BatchNumber);
        }

        var paginatedStock = await query
            .OrderBy(i => i.Item.ItemName)
            .ProjectTo<InventoryStockDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<InventoryStockDto>>.Success(paginatedStock);
    }
}
