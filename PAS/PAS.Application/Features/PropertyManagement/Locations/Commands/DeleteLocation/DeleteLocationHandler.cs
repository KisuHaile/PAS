using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.Locations.Commands.DeleteLocation;

public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteLocationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.PropertyLocations
            .Include(l => l.Properties)
            .Include(l => l.SafetyBoxes)
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            throw new NotFoundException(nameof(PropertyLocation), request.Id);
        }

        if (location.Properties?.Any(p => !p.IsDeleted) == true)
        {
            return Result.Failure("Cannot delete location with existing properties.");
        }

        if (location.SafetyBoxes?.Any(s => !s.IsDeleted) == true)
        {
            return Result.Failure("Cannot delete location with existing safety boxes.");
        }

        location.SoftDelete();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(PropertyLocation),
            location.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<PropertyLocation>(location, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}