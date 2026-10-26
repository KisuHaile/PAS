using Application.Features.Storage.InventoryStock.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Queries;

public class GetStockByItemQueryHandler : IRequestHandler<GetStockByItemQuery, Result<StockByItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStockByItemQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<StockByItemDto>> Handle(GetStockByItemQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.ItemMasters
            .FirstOrDefaultAsync(i => i.Id == request.ItemId && !i.IsDeleted, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ItemMaster), request.ItemId);
        }

        var stocks = await _context.InventoryStocks
            .Include(i => i.Shelf)
                .ThenInclude(s => s.Warehouse)
            .Where(i => i.ItemId == request.ItemId && !i.IsDeleted && i.CurrentQuantity > 0)
            .ToListAsync(cancellationToken);

        var result = new StockByItemDto
        {
            ItemId = item.Id,
            ItemName = item.ItemName,
            SKU = item.SKU,
            TotalQuantity = stocks.Sum(s => s.CurrentQuantity),
            ReservedQuantity = stocks.Sum(s => s.ReservedQuantity),
            Locations = stocks.Select(s => new StockLocationSummaryDto
            {
                ShelfId = s.ShelfId,
                ShelfLocation = $"{s.Shelf.Aisle}-{s.Shelf.Rack}-{s.Shelf.ShelfNumber}",
                WarehouseName = s.Shelf.Warehouse?.WarehouseName ?? "Unknown",
                Quantity = s.CurrentQuantity,
                ReservedQuantity = s.ReservedQuantity
            }).ToList()
        };

        return Result<StockByItemDto>.Success(result);
    }
}