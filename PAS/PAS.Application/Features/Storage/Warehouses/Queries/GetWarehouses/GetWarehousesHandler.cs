using Application.Features.Storage.Warehouses.Dtos;
using MediatR;

namespace Application.Features.Storage.Warehouses.Queries;

public class GetWarehousesQueryHandler : IRequestHandler<GetWarehousesQuery, Result<PaginatedList<WarehouseListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetWarehousesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<WarehouseListDto>>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = _context.Warehouses
            .Where(w => !w.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            warehouses = warehouses.Where(w =>
                w.WarehouseName.Contains(request.SearchTerm) ||
                w.LocationCode.Contains(request.SearchTerm) ||
                (w.City != null && w.City.Contains(request.SearchTerm)));
        }

        if (request.IsActive.HasValue)
        {
            warehouses = warehouses.Where(w => w.IsActive == request.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            warehouses = warehouses.Where(w => w.City == request.City);
        }

        // Project with simple counts - use subqueries
        var projectedQuery = warehouses.Select(w => new WarehouseListDto
        {
            Id = w.Id,
            WarehouseName = w.WarehouseName,
            LocationCode = w.LocationCode,
            City = w.City ?? string.Empty,
            IsActive = w.IsActive,
            ShelfCount = _context.ShelfLocations.Count(s => s.WarehouseId == w.Id && !s.IsDeleted),
            ItemCount = _context.ShelfLocations
                .Where(s => s.WarehouseId == w.Id && !s.IsDeleted)
                .SelectMany(s => s.InventoryStocks.Where(i => !i.IsDeleted))
                .Count()
        });

        var paginatedWarehouses = await projectedQuery
            .OrderBy(w => w.WarehouseName)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<WarehouseListDto>>.Success(paginatedWarehouses);
    }
}
