using MediatR;

namespace Application.Features.Storage.Warehouses.Commands;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateWarehouseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.Warehouse), request.Id);
        }

        // Check for duplicate warehouse name
        var existingName = await _context.Warehouses
            .AnyAsync(w => w.WarehouseName == request.WarehouseName && w.Id != request.Id && !w.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result.Failure($"Warehouse with name '{request.WarehouseName}' already exists.");
        }

        // Check for duplicate location code
        var existingCode = await _context.Warehouses
            .AnyAsync(w => w.LocationCode == request.LocationCode && w.Id != request.Id && !w.IsDeleted, cancellationToken);

        if (existingCode)
        {
            return Result.Failure($"Warehouse with location code '{request.LocationCode}' already exists.");
        }

        // Create a copy for event
        var oldWarehouse = new Domain.Storage.Warehouse(warehouse.WarehouseName, warehouse.LocationCode);

        // Update properties
        typeof(Domain.Storage.Warehouse).GetProperty(nameof(Domain.Storage.Warehouse.WarehouseName))?.SetValue(warehouse, request.WarehouseName);
        typeof(Domain.Storage.Warehouse).GetProperty(nameof(Domain.Storage.Warehouse.LocationCode))?.SetValue(warehouse, request.LocationCode);
        typeof(Domain.Storage.Warehouse).GetProperty("Address")?.SetValue(warehouse, request.Address);
        typeof(Domain.Storage.Warehouse).GetProperty("City")?.SetValue(warehouse, request.City);
        typeof(Domain.Storage.Warehouse).GetProperty("Country")?.SetValue(warehouse, request.Country);
        typeof(Domain.Storage.Warehouse).GetProperty("ContactPerson")?.SetValue(warehouse, request.ContactPerson);
        typeof(Domain.Storage.Warehouse).GetProperty("ContactPhone")?.SetValue(warehouse, request.ContactPhone);
        typeof(Domain.Storage.Warehouse).GetProperty("ContactEmail")?.SetValue(warehouse, request.ContactEmail);
        typeof(Domain.Storage.Warehouse).GetProperty("IsActive")?.SetValue(warehouse, request.IsActive);

        warehouse.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "UPDATE",
            nameof(Domain.Storage.Warehouse),
            warehouse.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Storage.Warehouse>(warehouse, oldWarehouse, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}