using Application.Features.Catalog.ItemMasters.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Queries.GetItemMasters;

public class GetItemMastersHandler : IRequestHandler<GetItemMastersQuery, Result<PaginatedList<ItemMasterListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetItemMastersHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ItemMasterListDto>>> Handle(GetItemMastersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ItemMasters
            .Include(i => i.Category)
            .Include(i => i.InventoryStocks)
            .Where(i => !i.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(i =>
                i.ItemName.Contains(request.SearchTerm) ||
                i.SKU.Contains(request.SearchTerm));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(i => i.CategoryId == request.CategoryId);
        }

        if (request.RequiresInspection.HasValue)
        {
            query = query.Where(i => i.RequiresInspection == request.RequiresInspection);
        }

        var items = await query
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
            item.IsLowStock = item.AvailableStock <= (dbItem?.MinStockLevel ?? 10);
        }

        if (request.LowStockOnly == true)
        {
            items = items.Where(i => i.IsLowStock).ToList();
        }

        var totalCount = items.Count;
        var paginatedItems = items
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new PaginatedList<ItemMasterListDto>(
            paginatedItems,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PaginatedList<ItemMasterListDto>>.Success(result);
    }
}