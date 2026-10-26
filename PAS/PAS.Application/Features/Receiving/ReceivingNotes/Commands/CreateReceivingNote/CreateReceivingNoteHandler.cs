using MediatR;

namespace Application.Features.Receiving.ReceivingNotes.Commands;

public class CreateReceivingNoteCommandHandler : IRequestHandler<CreateReceivingNoteCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateReceivingNoteCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateReceivingNoteCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        // Check for duplicate GRN
        var existingGrn = await _context.ReceivingNotes
            .AnyAsync(r => r.GRNNumber == request.GRNNumber && !r.IsDeleted, cancellationToken);

        if (existingGrn)
        {
            return Result<Guid>.Failure($"Receiving note with GRN '{request.GRNNumber}' already exists.");
        }

        // Verify supplier exists
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId && !s.IsDeleted, cancellationToken);

        if (supplier == null)
        {
            return Result<Guid>.Failure($"Supplier with ID {request.SupplierId} not found.");
        }

        // Verify all items exist
        foreach (var item in request.Items)
        {
            var itemMaster = await _context.ItemMasters
                .FirstOrDefaultAsync(i => i.Id == item.ItemId && !i.IsDeleted, cancellationToken);

            if (itemMaster == null)
            {
                return Result<Guid>.Failure($"Item with ID {item.ItemId} not found.");
            }
        }

        var defaultShelfId = await _context.ShelfLocations
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.ShelfNumber)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (defaultShelfId == Guid.Empty)
        {
            return Result<Guid>.Failure("No shelf location exists. Create at least one shelf location before creating receiving notes.");
        }

        // Create receiving note
        var receivingNote = new Domain.Receiving.ReceivingNote(
            request.GRNNumber,
            request.SupplierId,
            _currentUser.UserGuid.Value);

        // Set additional properties
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.PONumber))?.SetValue(receivingNote, request.PONumber);
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.InvoiceNumber))?.SetValue(receivingNote, request.InvoiceNumber);
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.InvoiceDate))?.SetValue(receivingNote, request.InvoiceDate);
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.DeliveryNoteNumber))?.SetValue(receivingNote, request.DeliveryNoteNumber);
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.VehicleNumber))?.SetValue(receivingNote, request.VehicleNumber);
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.DriverName))?.SetValue(receivingNote, request.DriverName);
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.Remarks))?.SetValue(receivingNote, request.Remarks);

        _context.ReceivingNotes.Add(receivingNote);

        // Add items to receiving note (you would need a ReceivingNoteItem entity)
        // This is simplified - you should create a ReceivingNoteItem entity in Domain
        foreach (var item in request.Items)
        {
            // Create stock ledger entry for received items (pending inspection)
            var ledger = new StockLedger(
                item.ItemId,
                defaultShelfId,
                item.Quantity,
                "RECEIVED_PENDING_INSPECTION",
                receivingNote.Id);
            _context.StockLedgers.Add(ledger);
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Receiving.ReceivingNote),
            receivingNote.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Receiving.ReceivingNote>(receivingNote, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(receivingNote.Id);
    }
}