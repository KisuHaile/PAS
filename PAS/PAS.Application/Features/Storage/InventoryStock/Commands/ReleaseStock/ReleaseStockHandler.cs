using MediatR;

namespace Application.Features.Storage.InventoryStock.Commands;

public class ReleaseStockCommandHandler : IRequestHandler<ReleaseStockCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public ReleaseStockCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(ReleaseStockCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        // Get reserved stock for this item
        var stocks = await _context.InventoryStocks
            .Where(s => s.ItemId == request.ItemId &&
                       s.ReservedQuantity > 0 &&
                       !s.IsDeleted)
            .ToListAsync(cancellationToken);

        if (request.ShelfId.HasValue)
        {
            stocks = stocks.Where(s => s.ShelfId == request.ShelfId).ToList();
        }

        var totalReserved = stocks.Sum(s => s.ReservedQuantity);

        if (totalReserved < request.Quantity)
        {
            return Result.Failure($"Cannot release more than reserved. Reserved: {totalReserved}");
        }

        int remainingToRelease = request.Quantity;

        foreach (var stock in stocks)
        {
            if (remainingToRelease <= 0)
                break;

            int toRelease = Math.Min(stock.ReservedQuantity, remainingToRelease);

            if (toRelease > 0)
            {
                // Update reserved quantity
                typeof(Domain.Storage.InventoryStock).GetProperty(nameof(Domain.Storage.InventoryStock.ReservedQuantity))
                    ?.SetValue(stock, stock.ReservedQuantity - toRelease);

                // Create stock ledger entry for release
                var ledger = new Domain.Storage.StockLedger(
                    request.ItemId,
                    stock.ShelfId,
                    toRelease,
                    "RELEASED",
                    request.ReferenceId);

                typeof(Domain.Storage.StockLedger).GetProperty("ReferenceType")?.SetValue(ledger, request.ReferenceType);
                _context.StockLedgers.Add(ledger);

                remainingToRelease -= toRelease;
            }
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "RELEASE",
            "InventoryStock",
            request.ItemId);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}