using Application.Common.Security;
using Application.Features.Receiving.Inspections.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Receiving.Inspections.Queries.GetInspections;

[Authorize(Permissions = Permissions.Receiving.View)]
public record GetInspectionsQuery : IRequest<Result<PaginatedList<InspectionDto>>>
{
    public Guid? ReceivingNoteId { get; init; }
    public bool? IsPassed { get; init; }
    public Guid? InspectorId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string? SearchTerm { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetInspectionsHandler : IRequestHandler<GetInspectionsQuery, Result<PaginatedList<InspectionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInspectionsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<InspectionDto>>> Handle(GetInspectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InspectionLogs
            .Include(i => i.ReceivingNote)
            .Where(i => !i.IsDeleted)
            .AsNoTracking();

        if (request.ReceivingNoteId.HasValue)
        {
            query = query.Where(i => i.ReceivingNoteId == request.ReceivingNoteId);
        }

        if (request.IsPassed.HasValue)
        {
            query = query.Where(i => i.IsPassed == request.IsPassed);
        }

        if (request.InspectorId.HasValue)
        {
            query = query.Where(i => i.InspectorId == request.InspectorId);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(i => i.InspectionDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(i => i.InspectionDate <= request.ToDate);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(i =>
                i.ReceivingNote.GRNNumber.Contains(request.SearchTerm) ||
                i.DeviationNotes.Contains(request.SearchTerm));
        }

        var paginatedInspections = await query
            .OrderByDescending(i => i.InspectionDate)
            .ProjectTo<InspectionDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // Populate inspection items for each inspection
        foreach (var inspection in paginatedInspections.Items)
        {
            var items = await _context.StockLedgers
                .Include(l => l.Item)
                .Where(l => l.ReferenceId == inspection.Id &&
                           (l.TransactionType == "INSPECTED_ACCEPTED" || l.TransactionType == "INSPECTED_REJECTED"))
                .GroupBy(l => l.ItemId)
                .Select(g => new InspectionItemDto
                {
                    ItemId = g.Key,
                    ItemName = g.First().Item.ItemName,
                    SKU = g.First().Item.SKU,
                    AcceptedQuantity = g.Where(l => l.TransactionType == "INSPECTED_ACCEPTED").Sum(l => l.QuantityChange),
                    RejectedQuantity = g.Where(l => l.TransactionType == "INSPECTED_REJECTED").Sum(l => Math.Abs(l.QuantityChange)),
                    IsPassed = !g.Any(l => l.TransactionType == "INSPECTED_REJECTED")
                })
                .ToListAsync(cancellationToken);

            inspection.Items = items;
            inspection.ReceivedQuantity = items.Sum(i => i.AcceptedQuantity + i.RejectedQuantity);
        }

        return Result<PaginatedList<InspectionDto>>.Success(paginatedInspections);
    }
}