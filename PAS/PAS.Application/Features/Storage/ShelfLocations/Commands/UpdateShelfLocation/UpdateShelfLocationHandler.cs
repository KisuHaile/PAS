using MediatR;

namespace Application.Features.Storage.ShelfLocations.Commands;

public class UpdateShelfLocationCommandHandler : IRequestHandler<UpdateShelfLocationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateShelfLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateShelfLocationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var shelfLocation = await _context.ShelfLocations
            .Include(s => s.Warehouse)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (shelfLocation == null)
        {
            throw new NotFoundException(nameof(Domain.Storage.ShelfLocation), request.Id);
        }

        // Check for duplicate shelf location (excluding current)
        var existingShelf = await _context.ShelfLocations
            .AnyAsync(s => s.WarehouseId == shelfLocation.WarehouseId &&
                          s.Aisle == request.Aisle &&
                          s.Rack == request.Rack &&
                          s.ShelfNumber == request.ShelfNumber &&
                          s.Id != request.Id &&
                          !s.IsDeleted, cancellationToken);

        if (existingShelf)
        {
            return Result.Failure("Shelf location already exists in this warehouse.");
        }

        // Create a copy for event
        var oldShelf = new Domain.Storage.ShelfLocation(
            shelfLocation.WarehouseId,
            shelfLocation.Aisle,
            shelfLocation.Rack,
            shelfLocation.ShelfNumber);

        // Update QR code if location changed
        if (shelfLocation.Aisle != request.Aisle ||
            shelfLocation.Rack != request.Rack ||
            shelfLocation.ShelfNumber != request.ShelfNumber)
        {
            var qrCodeValue = $"{shelfLocation.Warehouse?.LocationCode}-{request.Aisle}-{request.Rack}-{request.ShelfNumber}";
            typeof(Domain.Storage.ShelfLocation).GetProperty(nameof(Domain.Storage.ShelfLocation.QRCodeValue))?.SetValue(shelfLocation, qrCodeValue);
        }

        // Update properties
        typeof(Domain.Storage.ShelfLocation).GetProperty(nameof(Domain.Storage.ShelfLocation.Aisle))?.SetValue(shelfLocation, request.Aisle);
        typeof(Domain.Storage.ShelfLocation).GetProperty(nameof(Domain.Storage.ShelfLocation.Rack))?.SetValue(shelfLocation, request.Rack);
        typeof(Domain.Storage.ShelfLocation).GetProperty(nameof(Domain.Storage.ShelfLocation.ShelfNumber))?.SetValue(shelfLocation, request.ShelfNumber);
        typeof(Domain.Storage.ShelfLocation).GetProperty("Zone")?.SetValue(shelfLocation, request.Zone);
        typeof(Domain.Storage.ShelfLocation).GetProperty("BinType")?.SetValue(shelfLocation, request.BinType);
        typeof(Domain.Storage.ShelfLocation).GetProperty("Length")?.SetValue(shelfLocation, request.Length);
        typeof(Domain.Storage.ShelfLocation).GetProperty("Width")?.SetValue(shelfLocation, request.Width);
        typeof(Domain.Storage.ShelfLocation).GetProperty("Height")?.SetValue(shelfLocation, request.Height);
        typeof(Domain.Storage.ShelfLocation).GetProperty("MaxWeight")?.SetValue(shelfLocation, request.MaxWeight);
        typeof(Domain.Storage.ShelfLocation).GetProperty("Capacity")?.SetValue(shelfLocation, request.Capacity);
        typeof(Domain.Storage.ShelfLocation).GetProperty("IsActive")?.SetValue(shelfLocation, request.IsActive);

        shelfLocation.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "UPDATE",
            nameof(Domain.Storage.ShelfLocation),
            shelfLocation.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Storage.ShelfLocation>(shelfLocation, oldShelf, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}
