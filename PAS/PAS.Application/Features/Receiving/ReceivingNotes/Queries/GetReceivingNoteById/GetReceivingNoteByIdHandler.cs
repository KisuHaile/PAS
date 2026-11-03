using Application.Features.Receiving.ReceivingNotes.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Queries;

public class GetReceivingNoteByIdQueryHandler : IRequestHandler<GetReceivingNoteByIdQuery, Result<ReceivingNoteDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetReceivingNoteByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<ReceivingNoteDetailDto>> Handle(GetReceivingNoteByIdQuery request, CancellationToken cancellationToken)
    {
        var receivingNote = await _context.ReceivingNotes
            .Include(r => r.Supplier)
            .Include(r => r.InspectionLog)
            .Include(r => r.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.IsDeleted, cancellationToken);

        if (receivingNote == null)
        {
            throw new NotFoundException(nameof(Domain.Receiving.ReceivingNote), request.Id);
        }

        var noteDto = _mapper.Map<ReceivingNoteDetailDto>(receivingNote);

        // Get items from stock ledger
        var items = await _context.StockLedgers
            .Include(l => l.Item)
            .Where(l => l.ReferenceId == request.Id &&
                       (l.TransactionType == "RECEIVED_PENDING_INSPECTION" ||
                        l.TransactionType == "RECEIVED"))
            .Select(l => new ReceivingItemDetailDto
            {
                ItemId = l.ItemId,
                ItemName = l.Item != null ? l.Item.ItemName : string.Empty,
                SKU = l.Item != null ? l.Item.SKU : string.Empty,
                ReceivedQuantity = l.QuantityChange,
                AcceptedQuantity = l.QuantityChange,
                RejectedQuantity = 0,
                UnitOfMeasure = l.Item != null ? l.Item.UnitOfMeasure : string.Empty,
                RequiresInspection = true
            })
            .ToListAsync(cancellationToken);

        noteDto.Items = items;

        // Add inspection info
        if (receivingNote.InspectionLog != null)
        {
            noteDto.Inspection = new InspectionSummaryDto
            {
                Id = receivingNote.InspectionLog.Id,
                InspectionDate = receivingNote.InspectionLog.InspectionDate,
                InspectorName = string.Empty,
                IsPassed = receivingNote.InspectionLog.IsPassed,
                DeviationNotes = receivingNote.InspectionLog.DeviationNotes,
                AcceptedQuantity = items.Sum(i => i.AcceptedQuantity),
                RejectedQuantity = items.Sum(i => i.RejectedQuantity)
            };
        }

        return Result<ReceivingNoteDetailDto>.Success(noteDto);
    }
}