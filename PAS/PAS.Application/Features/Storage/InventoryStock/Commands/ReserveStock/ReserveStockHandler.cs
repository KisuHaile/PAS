using MediatR;

namespace Application.Features.Storage.InventoryStock.Commands;

public class ReserveStockCommandHandler : IRequestHandler<ReserveStockCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public ReserveStockCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        // Get available stock for this item
        var stocks = await _context.InventoryStocks
            .Where(s => s.ItemId == request.ItemId &&
                       s.CurrentQuantity > s.ReservedQuantity &&
                       !s.IsDeleted)
            .OrderBy(s => s.ExpiryDate ?? DateTime.MaxValue)
            .ToListAsync(cancellationToken);

        if (request.ShelfId.HasValue)
        {
            stocks = stocks.Where(s => s.ShelfId == request.ShelfId).ToList();
        }

        var totalAvailable = stocks.Sum(s => s.CurrentQuantity - s.ReservedQuantity);

        if (totalAvailable < request.Quantity)
        {
            return Result.Failure($"Insufficient stock. Available: {totalAvailable}, Requested: {request.Quantity}");
        }

        int remainingToReserve = request.Quantity;

        foreach (var stock in stocks)
        {
            if (remainingToReserve <= 0)
                break;

            int availableInStock = stock.CurrentQuantity - stock.ReservedQuantity;
            int toReserve = Math.Min(availableInStock, remainingToReserve);

            if (toReserve > 0)
            {
                // Update reserved quantity
                typeof(Domain.Storage.InventoryStock).GetProperty(nameof(Domain.Storage.InventoryStock.ReservedQuantity))
                    ?.SetValue(stock, stock.ReservedQuantity + toReserve);

                // Create stock ledger entry for reservation
                var ledger = new Domain.Storage.StockLedger(
                    request.ItemId,
                    stock.ShelfId,
                    -toReserve,
                    "RESERVED",
                    request.ReferenceId);

                typeof(Domain.Storage.StockLedger).GetProperty("ReferenceType")?.SetValue(ledger, request.ReferenceType);
                _context.StockLedgers.Add(ledger);

                remainingToReserve -= toReserve;
            }
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "RESERVE",
            "InventoryStock",
            request.ItemId);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}