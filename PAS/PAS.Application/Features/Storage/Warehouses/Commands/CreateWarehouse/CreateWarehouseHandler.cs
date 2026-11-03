using MediatR;

namespace Application.Features.Storage.Warehouses.Commands;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateWarehouseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        // Check for duplicate warehouse name
        var existingName = await _context.Warehouses
            .AnyAsync(w => w.WarehouseName == request.WarehouseName && !w.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Warehouse with name '{request.WarehouseName}' already exists.");
        }

        // Check for duplicate location code
        var existingCode = await _context.Warehouses
            .AnyAsync(w => w.LocationCode == request.LocationCode && !w.IsDeleted, cancellationToken);

        if (existingCode)
        {
            return Result<Guid>.Failure($"Warehouse with location code '{request.LocationCode}' already exists.");
        }

        var warehouse = new Domain.Storage.Warehouse(request.WarehouseName, request.LocationCode);

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.Address))
            typeof(Domain.Storage.Warehouse).GetProperty("Address")?.SetValue(warehouse, request.Address);

        if (!string.IsNullOrWhiteSpace(request.City))
            typeof(Domain.Storage.Warehouse).GetProperty("City")?.SetValue(warehouse, request.City);

        if (!string.IsNullOrWhiteSpace(request.Country))
            typeof(Domain.Storage.Warehouse).GetProperty("Country")?.SetValue(warehouse, request.Country);

        if (!string.IsNullOrWhiteSpace(request.ContactPerson))
            typeof(Domain.Storage.Warehouse).GetProperty("ContactPerson")?.SetValue(warehouse, request.ContactPerson);

        if (!string.IsNullOrWhiteSpace(request.ContactPhone))
            typeof(Domain.Storage.Warehouse).GetProperty("ContactPhone")?.SetValue(warehouse, request.ContactPhone);

        if (!string.IsNullOrWhiteSpace(request.ContactEmail))
            typeof(Domain.Storage.Warehouse).GetProperty("ContactEmail")?.SetValue(warehouse, request.ContactEmail);

        typeof(Domain.Storage.Warehouse).GetProperty("IsActive")?.SetValue(warehouse, true);

        _context.Warehouses.Add(warehouse);

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Storage.Warehouse),
            warehouse.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Storage.Warehouse>(warehouse, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(warehouse.Id);
    }
}
