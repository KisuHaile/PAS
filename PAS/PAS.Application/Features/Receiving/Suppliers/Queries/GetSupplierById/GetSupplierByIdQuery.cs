using Application.Common.Security;
using Application.Features.Receiving.Suppliers.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Receiving.Suppliers.Queries.GetSupplierById;

[Authorize(Permissions = Permissions.Suppliers.View)]
public record GetSupplierByIdQuery(Guid Id) : IRequest<Result<SupplierDetailDto>>;

public class GetSupplierByIdHandler : IRequestHandler<GetSupplierByIdQuery, Result<SupplierDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSupplierByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<SupplierDetailDto>> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.ReceivingNotes.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.StockLedgers)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (supplier == null)
        {
            throw new NotFoundException(nameof(Domain.Receiving.Supplier), request.Id);
        }

        var supplierDto = _mapper.Map<SupplierDetailDto>(supplier);

        // Populate receiving notes
        supplierDto.ReceivingNotes = new List<SupplierReceivingNoteDto>();

        foreach (var note in supplier.ReceivingNotes.Where(r => !r.IsDeleted))
        {
            var itemsReceived = await _context.StockLedgers
                .Where(l => l.ReferenceId == note.Id && l.TransactionType == "RECEIVED")
                .ToListAsync(cancellationToken);

            supplierDto.ReceivingNotes.Add(new SupplierReceivingNoteDto
            {
                Id = note.Id,
                GRNNumber = note.GRNNumber,
                ReceivedDate = note.ReceivedDate,
                Status = note.Status,
                ReceivedBy = string.Empty,
                ItemsCount = itemsReceived.Count,
                TotalQuantity = itemsReceived.Sum(l => l.QuantityChange > 0 ? l.QuantityChange : 0),
                TotalValue = 0 // Would need unit price from items
            });
        }

        // Order by most recent
        supplierDto.ReceivingNotes = supplierDto.ReceivingNotes
            .OrderByDescending(r => r.ReceivedDate)
            .ToList();

        return Result<SupplierDetailDto>.Success(supplierDto);
    }
}