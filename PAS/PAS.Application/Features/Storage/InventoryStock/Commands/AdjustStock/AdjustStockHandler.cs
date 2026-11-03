using MediatR;

namespace Application.Features.Storage.InventoryStock.Commands;

public class AdjustStockCommandHandler : IRequestHandler<AdjustStockCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public AdjustStockCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var inventory = await _context.InventoryStocks
            .Include(i => i.Item)
            .Include(i => i.ShelfLocation)
            .FirstOrDefaultAsync(i => i.Id == request.InventoryId && !i.IsDeleted, cancellationToken);

        if (inventory == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.InventoryStock), request.InventoryId);
        }

        if (request.NewQuantity < 0)
        {
            return Result.Failure("New quantity cannot be negative.");
        }

        int quantityChange = request.NewQuantity - inventory.CurrentQuantity;

        // Create a copy for event
        var oldInventory = new Domain.Storage.InventoryStock(
            inventory.ItemId,
            inventory.ShelfId,
            inventory.CurrentQuantity);

        // Update quantity
        typeof(Domain.Storage.InventoryStock).GetProperty(nameof(Domain.Storage.InventoryStock.CurrentQuantity))
            ?.SetValue(inventory, request.NewQuantity);

        inventory.MarkUpdated();

        // Create stock ledger entry for adjustment
        var ledger = new Domain.Storage.StockLedger(
            inventory.ItemId,
            inventory.ShelfId,
            quantityChange,
            "ADJUSTMENT",
            inventory.Id);

        typeof(Domain.Storage.StockLedger).GetProperty("Reason")?.SetValue(ledger, request.Reason);
        typeof(Domain.Storage.StockLedger).GetProperty("Remarks")?.SetValue(ledger, request.Remarks);
        _context.StockLedgers.Add(ledger);

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "ADJUST",
            nameof(Domain.Storage.InventoryStock),
            inventory.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Storage.InventoryStock>(inventory, oldInventory, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}
