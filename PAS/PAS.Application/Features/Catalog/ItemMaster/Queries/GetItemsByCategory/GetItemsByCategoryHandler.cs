using Application.Features.Catalog.ItemMasters.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetItemsByCategory;

public class GetItemsByCategoryHandler : IRequestHandler<GetItemsByCategoryQuery, Result<List<ItemMasterListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetItemsByCategoryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<ItemMasterListDto>>> Handle(GetItemsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.ItemMasters
            .Include(i => i.Category)
            .Include(i => i.InventoryStocks)
            .Where(i => i.CategoryId == request.CategoryId && !i.IsDeleted)
            .OrderBy(i => i.ItemName)
            .ProjectTo<ItemMasterListDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var dbItem = await _context.ItemMasters
                .Include(i => i.InventoryStocks)
                .FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken);

            var totalStock = dbItem?.InventoryStocks?.Sum(s => s.CurrentQuantity) ?? 0;
            var reservedStock = dbItem?.InventoryStocks?.Sum(s => s.ReservedQuantity) ?? 0;

            item.CurrentStock = totalStock;
            item.ReservedStock = reservedStock;
            item.AvailableStock = totalStock - reservedStock;
            item.IsLowStock = item.AvailableStock <= (dbItem?.MinStockLevel ?? 0);
        }

        return Result<List<ItemMasterListDto>>.Success(items);
    }
}