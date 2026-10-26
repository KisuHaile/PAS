using AutoMapper;
using MediatR;
using Application.Features.Storage.StockLedger.Dtos;

namespace Application.Features.Storage.StockLedger.Queries;

public class GetStockLedgerQueryHandler : IRequestHandler<GetStockLedgerQuery, Result<PaginatedList<StockLedgerDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTimeService _dateTime;

    public GetStockLedgerQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IDateTimeService dateTime)
    {
        _context = context;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<Result<PaginatedList<StockLedgerDto>>> Handle(GetStockLedgerQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockLedgers
            .Include(l => l.Item)
            .Include(l => l.Shelf)
                .ThenInclude(s => s.Warehouse)
            .Where(l => !l.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (request.ItemId.HasValue)
        {
            query = query.Where(l => l.ItemId == request.ItemId);
        }

        if (request.ShelfId.HasValue)
        {
            query = query.Where(l => l.ShelfId == request.ShelfId);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(l => l.Shelf.WarehouseId == request.WarehouseId);
        }

        if (!string.IsNullOrWhiteSpace(request.TransactionType))
        {
            query = query.Where(l => l.TransactionType == request.TransactionType);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(l => l.CreatedDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(l => l.CreatedDate <= request.ToDate);
        }

        if (request.ReferenceId.HasValue)
        {
            query = query.Where(l => l.ReferenceId == request.ReferenceId);
        }

        if (!string.IsNullOrWhiteSpace(request.BatchNumber))
        {
            query = query.Where(l => l.BatchNumber == request.BatchNumber);
        }

        var paginatedLedger = await query
            .OrderByDescending(l => l.CreatedDate)
            .ProjectTo<StockLedgerDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<StockLedgerDto>>.Success(paginatedLedger);
    }
}
