using MediatR;

namespace Application.Features.Receiving.Inspections.Commands.CreateInspection;

public class CreateInspectionCommandHandler : IRequestHandler<CreateInspectionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateInspectionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateInspectionCommand request, CancellationToken cancellationToken)
    {
        // Validate receiving note exists and is in pending inspection status
        var receivingNote = await _context.ReceivingNotes
            .Include(r => r.Supplier)
            .FirstOrDefaultAsync(r => r.Id == request.ReceivingNoteId && !r.IsDeleted, cancellationToken);

        if (receivingNote == null)
        {
            return Result<Guid>.Failure("Receiving note not found.");
        }

        if (receivingNote.Status != "PendingInspection")
        {
            return Result<Guid>.Failure($"Cannot inspect receiving note with status '{receivingNote.Status}'.");
        }

        // Check if inspection already exists
        var existingInspection = await _context.InspectionLogs
            .AnyAsync(i => i.ReceivingNoteId == request.ReceivingNoteId, cancellationToken);

        if (existingInspection)
        {
            return Result<Guid>.Failure("Inspection already exists for this receiving note.");
        }

        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("Current user not found.");
        }

        // Validate items match receiving note items
        var receivingItems = await _context.StockLedgers
            .Where(l => l.ReferenceId == receivingNote.Id &&
                        (l.TransactionType == "RECEIVED" || l.TransactionType == "RECEIVED_PENDING_INSPECTION"))
            .ToListAsync(cancellationToken);

        foreach (var item in request.Items)
        {
            var receivingItem = receivingItems.FirstOrDefault(i => i.ItemId == item.ItemId);
            if (receivingItem == null)
            {
                return Result<Guid>.Failure($"Item {item.ItemId} not found in receiving note.");
            }

            if (item.ReceivedQuantity != receivingItem.QuantityChange)
            {
                return Result<Guid>.Failure($"Received quantity mismatch for item {item.ItemId}. Expected: {receivingItem.QuantityChange}, Got: {item.ReceivedQuantity}");
            }
        }

        // Create inspection log
        var inspection = new Domain.Receiving.InspectionLog(
            request.ReceivingNoteId,
            _currentUser.UserGuid.Value,
            request.IsPassed,
            request.DeviationNotes);

        _context.InspectionLogs.Add(inspection);
        await _context.SaveChangesAsync(cancellationToken);

        // Process each inspected item
        var totalAccepted = 0;
        var totalRejected = 0;

        foreach (var item in request.Items)
        {
            if (item.AcceptedQuantity > 0)
            {
                // Add accepted items to inventory
                var availableShelves = await _context.ShelfLocations
                    .Where(s => !s.IsDeleted)
                    .OrderBy(s => s.ShelfNumber)
                    .ToListAsync(cancellationToken);

                var quantityToAdd = item.AcceptedQuantity;
                foreach (var shelf in availableShelves)
                {
                    if (quantityToAdd <= 0) break;

                    var inventoryStock = await _context.InventoryStocks
                        .FirstOrDefaultAsync(s => s.ItemId == item.ItemId && s.ShelfId == shelf.Id && !s.IsDeleted, cancellationToken);

                    if (inventoryStock == null)
                    {
                        inventoryStock = new InventoryStock(item.ItemId, shelf.Id, 0);
                        _context.InventoryStocks.Add(inventoryStock);
                    }

                    var addQuantity = Math.Min(quantityToAdd, 100); // Max per shelf, configurable
                    typeof(InventoryStock).GetProperty(nameof(InventoryStock.CurrentQuantity))?
                        .SetValue(inventoryStock, inventoryStock.CurrentQuantity + addQuantity);
                    inventoryStock.MarkUpdated();

                    // Create stock ledger entry for accepted items
                    var stockLedger = new StockLedger(
                        item.ItemId,
                        shelf.Id,
                        addQuantity,
                        "INSPECTED_ACCEPTED",
                        inspection.Id);
                    _context.StockLedgers.Add(stockLedger);

                    quantityToAdd -= addQuantity;
                    totalAccepted += addQuantity;
                }

                if (quantityToAdd > 0)
                {
                    return Result<Guid>.Failure($"Insufficient shelf space for accepted items of item {item.ItemId}. Remaining: {quantityToAdd}");
                }
            }

            if (item.RejectedQuantity > 0)
            {
                // Handle rejected items - create return to supplier record
                var returnRequest = new ReturnMaterialRequestNote(
                    item.ItemId,
                    item.RejectedQuantity,
                    $"Rejected during inspection: {item.Remarks ?? "Quality issues"}");
                _context.ReturnMaterialRequestNotes.Add(returnRequest);

                var receivingShelfId = receivingItems
                    .FirstOrDefault(i => i.ItemId == item.ItemId)?.ShelfId ?? Guid.Empty;

                if (receivingShelfId == Guid.Empty)
                {
                    return Result<Guid>.Failure($"No valid shelf location found for rejected item {item.ItemId}.");
                }

                // Create stock ledger entry for rejected items
                var stockLedger = new StockLedger(
                    item.ItemId,
                    receivingShelfId,
                    -item.RejectedQuantity,
                    "INSPECTED_REJECTED",
                    inspection.Id);
                _context.StockLedgers.Add(stockLedger);

                totalRejected += item.RejectedQuantity;
            }
        }

        // Update receiving note status
        var newStatus = request.IsPassed ? "Approved" : "Rejected";
        typeof(Domain.Receiving.ReceivingNote).GetProperty(nameof(Domain.Receiving.ReceivingNote.Status))?
            .SetValue(receivingNote, newStatus);
        receivingNote.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "INSPECT",
            nameof(Domain.Receiving.ReceivingNote),
            receivingNote.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        // Create notifications
        await CreateNotifications(receivingNote, inspection, totalAccepted, totalRejected, cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Receiving.InspectionLog>(inspection, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(inspection.Id);
    }

    private async Task CreateNotifications(
        Domain.Receiving.ReceivingNote receivingNote,
        Domain.Receiving.InspectionLog inspection,
        int totalAccepted,
        int totalRejected,
        CancellationToken cancellationToken)
    {
        // Notify receiver
        var receiverNotification = new Notification(
            receivingNote.ReceivedById,
            $"Inspection completed for GRN #{receivingNote.GRNNumber}. Accepted: {totalAccepted}, Rejected: {totalRejected}. Status: {(inspection.IsPassed ? "Approved" : "Rejected")}");
        _context.Notifications.Add(receiverNotification);

        // Notify store officer for approved items
        if (inspection.IsPassed && totalAccepted > 0)
        {
            // Store officer broadcast is skipped here after removing legacy UserLogin model.
        }

        // Notify supplier for rejected items (if email service available)
        if (totalRejected > 0 && !string.IsNullOrWhiteSpace(receivingNote.Supplier?.Email))
        {
            // Would send email notification
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}