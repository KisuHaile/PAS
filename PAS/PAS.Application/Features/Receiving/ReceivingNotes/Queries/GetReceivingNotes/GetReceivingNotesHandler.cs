using Application.Features.Receiving.ReceivingNotes.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Queries;

public class GetReceivingNotesQueryHandler : IRequestHandler<GetReceivingNotesQuery, Result<PaginatedList<ReceivingNoteListDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReceivingNotesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<ReceivingNoteListDto>>> Handle(GetReceivingNotesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReceivingNotes
            .Include(r => r.Supplier)
            .Include(r => r.InspectionLog)
            .Where(r => !r.IsDeleted)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(r => r.Status == request.Status);
        }

        if (request.SupplierId.HasValue)
        {
            query = query.Where(r => r.SupplierId == request.SupplierId);
        }

        if (request.ReceivedById.HasValue)
        {
            query = query.Where(r => r.ReceivedById == request.ReceivedById);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(r => r.ReceivedDate >= request.FromDate);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(r => r.ReceivedDate <= request.ToDate);
        }

        if (request.HasInspection.HasValue)
        {
            if (request.HasInspection.Value)
                query = query.Where(r => r.InspectionLog != null);
            else
                query = query.Where(r => r.InspectionLog == null);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(r =>
                r.GRNNumber.Contains(request.SearchTerm) ||
                (r.Supplier != null && r.Supplier.SupplierName.Contains(request.SearchTerm)) ||
                (r.PONumber != null && r.PONumber.Contains(request.SearchTerm)) ||
                (r.InvoiceNumber != null && r.InvoiceNumber.Contains(request.SearchTerm)));
        }

        // Project to DTO
        var projectedQuery = query.Select(r => new ReceivingNoteListDto
        {
            Id = r.Id,
            GRNNumber = r.GRNNumber,
            SupplierName = r.Supplier != null ? r.Supplier.SupplierName : string.Empty,
            ReceivedDate = r.ReceivedDate,
            Status = r.Status,
            ReceivedBy = string.Empty,
            ItemCount = 0, // Would need to calculate from items
            TotalQuantity = 0, // Would need to calculate from items
            HasInspection = r.InspectionLog != null
        });

        var paginatedNotes = await projectedQuery
            .OrderByDescending(r => r.ReceivedDate)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return Result<PaginatedList<ReceivingNoteListDto>>.Success(paginatedNotes);
    }
}
