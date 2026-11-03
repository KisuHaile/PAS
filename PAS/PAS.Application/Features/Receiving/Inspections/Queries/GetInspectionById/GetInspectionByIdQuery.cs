using Application.Common.Security;
using Application.Features.Receiving.Inspections.Dtos;
using AutoMapper;
using MediatR;

namespace Application.Features.Receiving.Inspections.Queries.GetInspectionById;

[Authorize(Permissions = Permissions.Receiving.View)]
public record GetInspectionByIdQuery(Guid Id) : IRequest<Result<InspectionDetailDto>>;

public class GetInspectionByIdHandler : IRequestHandler<GetInspectionByIdQuery, Result<InspectionDetailDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInspectionByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<InspectionDetailDto>> Handle(GetInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        var inspection = await _context.InspectionLogs
            .Include(i => i.ReceivingNote)
                .ThenInclude(r => r.Supplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (inspection == null)
        {
            throw new NotFoundException(nameof(Domain.Receiving.InspectionLog), request.Id);
        }

        var inspectionDto = _mapper.Map<InspectionDetailDto>(inspection);

        // Get inspection items
        var stockLedgers = await _context.StockLedgers
            .Include(l => l.Item)
            .Where(l => l.ReferenceId == inspection.Id &&
                       (l.TransactionType == "INSPECTED_ACCEPTED" || l.TransactionType == "INSPECTED_REJECTED"))
            .ToListAsync(cancellationToken);

        var itemsByItem = stockLedgers.GroupBy(l => l.ItemId);

        foreach (var group in itemsByItem)
        {
            var item = group.First().Item;
            var acceptedQty = group.Where(l => l.TransactionType == "INSPECTED_ACCEPTED").Sum(l => l.QuantityChange);
            var rejectedQty = group.Where(l => l.TransactionType == "INSPECTED_REJECTED").Sum(l => Math.Abs(l.QuantityChange));

            inspectionDto.Items.Add(new InspectionItemDto
            {
                ItemId = item.Id,
                ItemName = item.ItemName,
                SKU = item.SKU,
                ReceivedQuantity = acceptedQty + rejectedQty,
                AcceptedQuantity = acceptedQty,
                RejectedQuantity = rejectedQty,
                Remarks = inspection.DeviationNotes,
                IsPassed = rejectedQty == 0
            });
        }

        // Populate receiving note summary
        var receivingNote = inspection.ReceivingNote;
        var totalReceived = await _context.StockLedgers
            .Where(l => l.ReferenceId == receivingNote.Id && l.TransactionType == "RECEIVED")
            .SumAsync(l => l.QuantityChange, cancellationToken);

        inspectionDto.ReceivingNoteSummary = new ReceivingNoteSummaryDto
        {
            Id = receivingNote.Id,
            GRNNumber = receivingNote.GRNNumber,
            SupplierName = receivingNote.Supplier?.SupplierName ?? "Unknown",
            ReceivedDate = receivingNote.ReceivedDate,
            ReceivedBy = string.Empty,
            TotalItems = inspectionDto.Items.Count,
            TotalQuantity = totalReceived
        };

        // Get attachments (if any)
        var attachments = await _context.DocumentAttachments
            .Where(a => a.RelatedEntityId == inspection.Id && a.RelatedEntityName == nameof(Domain.Receiving.InspectionLog) && !a.IsDeleted)
            .Select(a => new InspectionAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                UploadedAt = a.CreatedAt,
                UploadedBy = "System"
            })
            .ToListAsync(cancellationToken);

        inspectionDto.Attachments = attachments;

        return Result<InspectionDetailDto>.Success(inspectionDto);
    }
}