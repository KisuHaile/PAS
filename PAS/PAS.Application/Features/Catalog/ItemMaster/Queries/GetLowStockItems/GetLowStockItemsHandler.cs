using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Catalog.ItemMasters.Queries.GetLowStockItems;

public class GetLowStockItemsHandler : IRequestHandler<GetLowStockItemsQuery, Result<List<LowStockItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockItemsHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<LowStockItemDto>>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
    {
        var lowStockItems = await _context.ItemMasters
            .Where(i => !i.IsDeleted)
            .Select(i => new
            {
                Item = i,
                CurrentStock = i.InventoryStocks.Sum(s => (int?)s.CurrentQuantity) ?? 0
            })
            .Where(x => x.CurrentStock <= x.Item.MinStockLevel) // MinStockLevel = 10 works here
            .Select(x => new LowStockItemDto
            {
                ItemId = x.Item.Id,
                ItemName = x.Item.ItemName,
                SKU = x.Item.SKU,
                CurrentStock = x.CurrentStock,
                MinStockLevel = x.Item.MinStockLevel,
                Deficit = x.Item.MinStockLevel - x.CurrentStock,
                Locations = x.Item.InventoryStocks
                    .Where(s => s.CurrentQuantity > 0)
                    .Select(s => s.Shelf.Aisle + "-" + s.Shelf.Rack + "-" + s.Shelf.ShelfNumber)
                    .ToList()
            })
            .OrderByDescending(i => i.Deficit) // biggest shortage first (better UX)
            .ToListAsync(cancellationToken);

        return Result<List<LowStockItemDto>>.Success(lowStockItems);
    }
}
