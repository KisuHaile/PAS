using Application.Features.Storage.StockLedger.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Storage.StockLedger.Queries;

public class GetStockMovementsByItemQueryHandler : IRequestHandler<GetStockMovementsByItemQuery, Result<StockMovementSummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTimeService _dateTime;

    public GetStockMovementsByItemQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IDateTimeService dateTime)
    {
        _context = context;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<Result<StockMovementSummaryDto>> Handle(GetStockMovementsByItemQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockLedgers
            .Where(l => l.ItemId == request.ItemId && !l.IsDeleted);

        if (request.FromDate.HasValue)
        {
            query = query.Where(l => l.CreatedDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(l => l.CreatedDate <= request.ToDate);
        }

        var movements = await query.ToListAsync(cancellationToken);

        var summary = new StockMovementSummaryDto
        {
            TotalMovements = movements.Count,
            TotalIn = movements.Count(m => m.QuantityChange > 0),
            TotalOut = movements.Count(m => m.QuantityChange < 0),
            TotalAdjustments = movements.Count(m => m.TransactionType == "ADJUSTMENT"),
            QuantityIn = movements.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
            QuantityOut = Math.Abs(movements.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
            NetChange = movements.Sum(m => m.QuantityChange)
        };

        return Result<StockMovementSummaryDto>.Success(summary);
    }
}