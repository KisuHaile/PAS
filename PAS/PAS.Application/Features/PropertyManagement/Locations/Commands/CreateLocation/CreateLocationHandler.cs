using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Commands.CreateLocation;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var existingName = await _context.PropertyLocations
            .AnyAsync(l => l.Name == request.Name && !l.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Location with name '{request.Name}' already exists.");
        }

        var location = new PropertyLocation(request.Name, request.LocationType);

        _context.PropertyLocations.Add(location);
        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(PropertyLocation),
            location.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<PropertyLocation>(location, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(location.Id);
    }
}