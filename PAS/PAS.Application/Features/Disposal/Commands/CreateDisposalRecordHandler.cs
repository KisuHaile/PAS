using MediatR;

namespace Application.Features.Disposal.Commands.CreateDisposalRecord;

public class CreateDisposalRecordCommandHandler : IRequestHandler<CreateDisposalRecordCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateDisposalRecordCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateDisposalRecordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("Current user not found.");
        }

        // Validate all items and check stock availability
        var itemsToDispose = new List<(Domain.Catalog.ItemMaster Item, int Quantity, string Reason, Guid ShelfId)>();
        var validationErrors = new List<string>();

        foreach (var itemDto in request.Items)
        {
            if (itemDto.Quantity <= 0)
            {
                validationErrors.Add($"Quantity must be greater than zero for item {itemDto.ItemId}.");
                continue;
            }

            var item = await _context.ItemMasters
                .FirstOrDefaultAsync(i => i.Id == itemDto.ItemId && !i.IsDeleted, cancellationToken);

            if (item == null)
            {
                validationErrors.Add($"Item with ID {itemDto.ItemId} not found.");
                continue;
            }

            // Check available stock
            var availableStock = await _context.InventoryStocks
                .Where(s => s.ItemId == item.Id && !s.IsDeleted)
                .SumAsync(s => s.CurrentQuantity - s.ReservedQuantity, cancellationToken);

            if (availableStock < itemDto.Quantity)
            {
                validationErrors.Add($"Insufficient stock for item '{item.ItemName}'. Available: {availableStock}, Requested for disposal: {itemDto.Quantity}");
                continue;
            }

            var shelfId = await _context.InventoryStocks
                .Where(s => s.ItemId == item.Id && !s.IsDeleted && (s.CurrentQuantity - s.ReservedQuantity) > 0)
                .OrderByDescending(s => s.CurrentQuantity - s.ReservedQuantity)
                .Select(s => s.ShelfId)
                .FirstOrDefaultAsync(cancellationToken);

            if (shelfId == Guid.Empty)
            {
                validationErrors.Add($"No valid shelf location found for item '{item.ItemName}'.");
                continue;
            }

            var reason = string.IsNullOrWhiteSpace(itemDto.Reason) ? request.Reason : itemDto.Reason;
            itemsToDispose.Add((item, itemDto.Quantity, reason, shelfId));
        }

        if (validationErrors.Any())
        {
            return Result<Guid>.Failure(validationErrors.ToArray());
        }

        // Create disposal records
        var disposalRecords = new List<Domain.Disposal.DisposalRecord>();

        foreach (var (item, quantity, reason, shelfId) in itemsToDispose)
        {
            var disposalRecord = new Domain.Disposal.DisposalRecord(
                item.Id,
                quantity,
                _currentUser.UserGuid.Value,
                reason);

            _context.DisposalRecords.Add(disposalRecord);
            disposalRecords.Add(disposalRecord);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Create stock ledger entries (pending approval - will be processed on approval)
        foreach (var record in disposalRecords)
        {
            var shelfId = itemsToDispose
                .First(x => x.Item.Id == record.ItemId)
                .ShelfId;

            var stockLedger = new StockLedger(
                record.ItemId,
                shelfId,
                -record.Quantity,
                "DISPOSAL_PENDING",
                record.Id);
            _context.StockLedgers.Add(stockLedger);
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Disposal.DisposalRecord),
            disposalRecords.First().Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        // Notify approvers
        await CreateApproverNotifications(disposalRecords.First().Id, cancellationToken);

        // Publish event
        foreach (var record in disposalRecords)
        {
            await _mediator.Publish(new EntityCreatedEvent<Domain.Disposal.DisposalRecord>(record, _currentUser.UserGuid), cancellationToken);
        }

        return Result<Guid>.Success(disposalRecords.First().Id);
    }

    private async Task CreateApproverNotifications(Guid disposalId, CancellationToken cancellationToken)
    {
        // Get all approvers for disposal workflow
        var approvers = await _context.Approvers
            .Where(a => a.Workflow.WorkflowName == "DisposalApproval" && !a.IsDeleted)
            .OrderBy(a => a.ApprovalLevel)
            .ToListAsync(cancellationToken);

        foreach (var approver in approvers)
        {
            var notification = new Notification(
                approver.UserId,
                $"New disposal request #{disposalId} requires your approval.");
            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}