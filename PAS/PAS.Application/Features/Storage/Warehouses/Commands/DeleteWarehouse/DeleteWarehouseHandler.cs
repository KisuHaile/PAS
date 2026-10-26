using MediatR;

namespace Application.Features.Storage.Warehouses.Commands;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteWarehouseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var warehouse = await _context.Warehouses
            .Include(w => w.ShelfLocations)
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.Warehouse), request.Id);
        }

        // Check if warehouse has active shelf locations
        if (warehouse.ShelfLocations?.Any(s => !s.IsDeleted) == true)
        {
            return Result.Failure("Cannot delete warehouse with existing shelf locations. Delete or move the shelves first.");
        }

        warehouse.SoftDelete();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "DELETE",
            nameof(Domain.Storage.Warehouse),
            warehouse.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Domain.Storage.Warehouse>(warehouse, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}