using Application.Features.Storage.InventoryStock.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Storage.InventoryStock.Queries;

public class GetStockByShelfQueryHandler : IRequestHandler<GetStockByShelfQuery, Result<List<InventoryStockDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStockByShelfQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<InventoryStockDto>>> Handle(GetStockByShelfQuery request, CancellationToken cancellationToken)
    {
        var stock = await _context.InventoryStocks
            .Include(i => i.Item)
            .Include(i => i.ShelfLocation)
            .Where(i => i.ShelfId == request.ShelfId && !i.IsDeleted && i.CurrentQuantity > 0)
            .OrderBy(i => i.Item.ItemName)
            .ProjectTo<InventoryStockDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return Result<List<InventoryStockDto>>.Success(stock);
    }
}
