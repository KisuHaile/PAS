using Application.Common.Security;
using Application.Features.Receiving.Suppliers.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Receiving.Suppliers.Queries.GetSuppliers;

[Authorize(Permissions = Permissions.Suppliers.View)]
public record GetSuppliersQuery : IRequest<Result<PaginatedList<SupplierDto>>>
{
    public string? SearchTerm { get; init; }
    public bool? IsActive { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetSuppliersHandler : IRequestHandler<GetSuppliersQuery, Result<PaginatedList<SupplierDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSuppliersHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PaginatedList<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Suppliers
            .Include(s => s.ReceivingNotes.Where(r => !r.IsDeleted))
            .Where(s => !s.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(s =>
                s.SupplierName.Contains(request.SearchTerm) ||
                s.ContactPerson.Contains(request.SearchTerm) ||
                s.TinNumber.Contains(request.SearchTerm) ||
                (s.Email != null && s.Email.Contains(request.SearchTerm)));
        }

        var isActiveProperty = typeof(Domain.Receiving.Supplier).GetProperty("IsActive");
        if (request.IsActive.HasValue && isActiveProperty != null)
        {
            // Filter by IsActive if the property exists
            query = query.Where(s => EF.Property<bool>(s, "IsActive") == request.IsActive);
        }

        var paginatedSuppliers = await query
            .OrderBy(s => s.SupplierName)
            .ProjectTo<SupplierDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // Calculate statistics for each supplier
        foreach (var supplier in paginatedSuppliers.Items)
        {
            var receivingNotes = await _context.ReceivingNotes
                .Where(r => r.SupplierId == supplier.Id && !r.IsDeleted)
                .Include(r => r.StockLedgers)
                .ToListAsync(cancellationToken);

            supplier.ReceivingNotesCount = receivingNotes.Count;
            supplier.TotalItemsReceived = receivingNotes
                .SelectMany(r => _context.StockLedgers.Where(l => l.ReferenceId == r.Id && l.TransactionType == "RECEIVED"))
                .Sum(l => l.QuantityChange > 0 ? l.QuantityChange : 0);

            // Calculate total purchase value (if unit price is stored)
            // This would require additional fields in StockLedger or ReceivingNote
        }

        return Result<PaginatedList<SupplierDto>>.Success(paginatedSuppliers);
    }
}