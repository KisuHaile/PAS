using Application.Features.Catalog.ItemMasters.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetItemMasterById;

public class GetItemMasterByIdHandler : IRequestHandler<GetItemMasterByIdQuery, Result<ItemMasterDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetItemMasterByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ItemMasterDetailDto>> Handle(GetItemMasterByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _context.ItemMasters
            .Include(i => i.Category)
            .Include(i => i.InventoryStocks)
                .ThenInclude(s => s.Shelf)
                    .ThenInclude(sh => sh.Warehouse)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ItemMaster), request.Id);
        }

        var itemDto = _mapper.Map<ItemMasterDetailDto>(item);

        itemDto.TotalStock = item.InventoryStocks?.Sum(s => s.CurrentQuantity) ?? 0;
        itemDto.AvailableStock = item.InventoryStocks?.Sum(s => s.CurrentQuantity - s.ReservedQuantity) ?? 0;

        itemDto.StockLocations = item.InventoryStocks?
            .Where(s => s.CurrentQuantity > 0)
            .Select(s => new ItemStockLocationDto
            {
                ShelfId = s.ShelfId,
                ShelfLocation = $"{s.Shelf.Aisle}-{s.Shelf.Rack}-{s.Shelf.ShelfNumber}",
                WarehouseName = s.Shelf.Warehouse?.WarehouseName ?? string.Empty,
                CurrentQuantity = s.CurrentQuantity,
                ReservedQuantity = s.ReservedQuantity,
                AvailableQuantity = s.CurrentQuantity - s.ReservedQuantity
            })
            .ToList() ?? new();

        var movements = await _context.StockLedgers
            .Include(l => l.Shelf)
            .Where(l => l.ItemId == request.Id)
            .OrderByDescending(l => l.CreatedDate)
            .Take(20)
            .Select(l => new ItemMovementDto
            {
                Date = l.CreatedDate,
                TransactionType = l.TransactionType,
                QuantityChange = l.QuantityChange,
                Reference = l.ReferenceId.ToString(),
                ShelfLocation = l.Shelf != null ? $"{l.Shelf.Aisle}-{l.Shelf.Rack}-{l.Shelf.ShelfNumber}" : "N/A"
            })
            .ToListAsync(cancellationToken);

        itemDto.RecentMovements = movements;

        return Result<ItemMasterDetailDto>.Success(itemDto);
    }
}