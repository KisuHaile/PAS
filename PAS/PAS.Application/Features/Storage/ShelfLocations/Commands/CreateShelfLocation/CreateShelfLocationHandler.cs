using MediatR;

namespace Application.Features.Storage.ShelfLocations.Commands;

public class CreateShelfLocationCommandHandler : IRequestHandler<CreateShelfLocationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateShelfLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateShelfLocationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        // Verify warehouse exists
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            return Result<Guid>.Failure($"Warehouse with ID {request.WarehouseId} not found.");
        }

        // Check for duplicate shelf location
        var existingShelf = await _context.ShelfLocations
            .AnyAsync(s => s.WarehouseId == request.WarehouseId &&
                          s.Aisle == request.Aisle &&
                          s.Rack == request.Rack &&
                          s.ShelfNumber == request.ShelfNumber &&
                          !s.IsDeleted, cancellationToken);

        if (existingShelf)
        {
            return Result<Guid>.Failure("Shelf location already exists in this warehouse.");
        }

        // Generate QR code
        var qrCodeValue = $"{warehouse.LocationCode}-{request.Aisle}-{request.Rack}-{request.ShelfNumber}";

        var shelfLocation = new Domain.Storage.ShelfLocation(
            request.WarehouseId,
            request.Aisle,
            request.Rack,
            request.ShelfNumber);

        typeof(Domain.Storage.ShelfLocation).GetProperty(nameof(Domain.Storage.ShelfLocation.QRCodeValue))?.SetValue(shelfLocation, qrCodeValue);

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.Zone))
            typeof(Domain.Storage.ShelfLocation).GetProperty("Zone")?.SetValue(shelfLocation, request.Zone);

        if (!string.IsNullOrWhiteSpace(request.BinType))
            typeof(Domain.Storage.ShelfLocation).GetProperty("BinType")?.SetValue(shelfLocation, request.BinType);

        if (request.Length.HasValue)
            typeof(Domain.Storage.ShelfLocation).GetProperty("Length")?.SetValue(shelfLocation, request.Length);

        if (request.Width.HasValue)
            typeof(Domain.Storage.ShelfLocation).GetProperty("Width")?.SetValue(shelfLocation, request.Width);

        if (request.Height.HasValue)
            typeof(Domain.Storage.ShelfLocation).GetProperty("Height")?.SetValue(shelfLocation, request.Height);

        if (request.MaxWeight.HasValue)
            typeof(Domain.Storage.ShelfLocation).GetProperty("MaxWeight")?.SetValue(shelfLocation, request.MaxWeight);

        typeof(Domain.Storage.ShelfLocation).GetProperty("Capacity")?.SetValue(shelfLocation, request.Capacity);
        typeof(Domain.Storage.ShelfLocation).GetProperty("IsActive")?.SetValue(shelfLocation, true);

        _context.ShelfLocations.Add(shelfLocation);

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Storage.ShelfLocation),
            shelfLocation.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Storage.ShelfLocation>(shelfLocation, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(shelfLocation.Id);
    }
}