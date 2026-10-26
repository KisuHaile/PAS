using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            throw new NotFoundException(nameof(PropertyLocation), request.Id);
        }

        var existingName = await _context.PropertyLocations
            .AnyAsync(l => l.Name == request.Name && l.Id != request.Id && !l.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result.Failure($"Location with name '{request.Name}' already exists.");
        }

        var oldLocation = new PropertyLocation(location.Name, location.LocationType);

        typeof(PropertyLocation).GetProperty(nameof(PropertyLocation.Name))?.SetValue(location, request.Name);
        typeof(PropertyLocation).GetProperty(nameof(PropertyLocation.LocationType))?.SetValue(location, request.LocationType);

        location.MarkUpdated();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(PropertyLocation),
            location.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<PropertyLocation>(location, oldLocation, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}