using MediatR;

namespace Application.Features.Storage.ShelfLocations.Commands;

public class DeleteShelfLocationCommandHandler : IRequestHandler<DeleteShelfLocationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteShelfLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteShelfLocationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var shelfLocation = await _context.ShelfLocations
            .Include(s => s.InventoryStocks)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (shelfLocation == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.ShelfLocation), request.Id);
        }

        // Check if shelf has inventory
        if (shelfLocation.InventoryStocks?.Any(i => i.CurrentQuantity > 0) == true)
        {
            return Result.Failure("Cannot delete shelf location with existing inventory. Move the items first.");
        }

        shelfLocation.SoftDelete();

        // Soft delete any inventory records (should be zero quantity by now)
        if (shelfLocation.InventoryStocks?.Any() == true)
        {
            foreach (var stock in shelfLocation.InventoryStocks.Where(i => !i.IsDeleted))
            {
                stock.SoftDelete();
            }
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "DELETE",
            nameof(Domain.Storage.ShelfLocation),
            shelfLocation.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Domain.Storage.ShelfLocation>(shelfLocation, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}