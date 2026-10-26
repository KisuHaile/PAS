using Application.Features.Storage.StockLedger.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Storage.StockLedger.Queries;

public class GetStockMovementsByDateQueryHandler : IRequestHandler<GetStockMovementsByDateQuery, Result<List<StockMovementByDateDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDateTimeService _dateTime;

    public GetStockMovementsByDateQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IDateTimeService dateTime)
    {
        _context = context;
        _mapper = mapper;
        _dateTime = dateTime;
    }

    public async Task<Result<List<StockMovementByDateDto>>> Handle(GetStockMovementsByDateQuery request, CancellationToken cancellationToken)
    {
        var query = _context.StockLedgers
            .Where(l => !l.IsDeleted &&
                       l.CreatedDate >= request.FromDate &&
                       l.CreatedDate <= request.ToDate);

        if (request.ItemId.HasValue)
        {
            query = query.Where(l => l.ItemId == request.ItemId);
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(l => l.Shelf.WarehouseId == request.WarehouseId);
        }

        var movements = await query.ToListAsync(cancellationToken);

        var groupedMovements = request.GroupBy?.ToLower() switch
        {
            "week" => movements
                .GroupBy(m => new {
                    Year = m.CreatedDate.Year,
                    Week = System.Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                        m.CreatedDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                })
                .Select(g => new StockMovementByDateDto
                {
                    Date = new DateTime(g.Key.Year, 1, 1).AddDays((g.Key.Week - 1) * 7),
                    In = g.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
                    Out = Math.Abs(g.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
                    Net = g.Sum(m => m.QuantityChange)
                })
                .OrderBy(d => d.Date)
                .ToList(),

            "month" => movements
                .GroupBy(m => new { m.CreatedDate.Year, m.CreatedDate.Month })
                .Select(g => new StockMovementByDateDto
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    In = g.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
                    Out = Math.Abs(g.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
                    Net = g.Sum(m => m.QuantityChange)
                })
                .OrderBy(d => d.Date)
                .ToList(),

            _ => movements
                .GroupBy(m => m.CreatedDate.Date)
                .Select(g => new StockMovementByDateDto
                {
                    Date = g.Key,
                    In = g.Where(m => m.QuantityChange > 0).Sum(m => m.QuantityChange),
                    Out = Math.Abs(g.Where(m => m.QuantityChange < 0).Sum(m => m.QuantityChange)),
                    Net = g.Sum(m => m.QuantityChange)
                })
                .OrderBy(d => d.Date)
                .ToList()
        };

        return Result<List<StockMovementByDateDto>>.Success(groupedMovements);
    }
}