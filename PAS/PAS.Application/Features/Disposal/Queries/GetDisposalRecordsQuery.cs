using Application.Common.Security;
using Application.Features.Disposal.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Disposal.Queries.GetDisposalRecords;

[Authorize(Permissions = Permissions.Disposal.View)]
public record GetDisposalRecordsQuery : IRequest<Result<PaginatedList<DisposalRecordDto>>>
{
    public string? Status { get; init; }
    public Guid? ItemId { get; init; }
    public Guid? DisposedBy { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetDisposalRecordsHandler : IRequestHandler<GetDisposalRecordsQuery, Result<PaginatedList<DisposalRecordDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDisposalRecordsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<DisposalRecordDto>>> Handle(GetDisposalRecordsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DisposalRecords
            .Include(d => d.Item)
            .Where(d => !d.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            // Determine status based on whether stock ledger shows DISPOSED or not
            if (request.Status == "Approved")
            {
                query = query.Where(d => _context.StockLedgers.Any(l => l.ReferenceId == d.Id && l.TransactionType == "DISPOSED"));
            }
            else if (request.Status == "Pending")
            {
                query = query.Where(d => !_context.StockLedgers.Any(l => l.ReferenceId == d.Id && l.TransactionType == "DISPOSED"));
            }
        }

        if (request.ItemId.HasValue)
        {
            query = query.Where(d => d.ItemId == request.ItemId);
        }

        if (request.DisposedBy.HasValue)
        {
            query = query.Where(d => d.DisposedById == request.DisposedBy);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(d => d.DisposalDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(d => d.DisposalDate <= request.ToDate);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(d => (d.Item != null && d.Item.ItemName.Contains(request.SearchTerm)) ||
                                    d.Reason.Contains(request.SearchTerm));
        }

        var paginatedRecords = await query
            .OrderByDescending(d => d.DisposalDate)
            .ProjectTo<DisposalRecordDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // Set status for each record
        foreach (var record in paginatedRecords.Items)
        {
            var hasDisposedLedger = await _context.StockLedgers
                .AnyAsync(l => l.ReferenceId == record.Id && l.TransactionType == "DISPOSED", cancellationToken);

            record.Status = hasDisposedLedger ? "Approved" : "Pending";

            // Calculate estimated value (based on average cost)
            var stockLedgers = await _context.StockLedgers
                .Where(l => l.ItemId == record.ItemId && l.TransactionType == "RECEIVED")
                .OrderByDescending(l => l.CreatedDate)
                .Take(10)
                .ToListAsync(cancellationToken);

            if (stockLedgers.Any())
            {
                record.EstimatedValue = stockLedgers.Average(l => (decimal)l.QuantityChange) * record.Quantity;
            }
        }

        return Result<PaginatedList<DisposalRecordDto>>.Success(paginatedRecords);
    }
}