using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.UpdateSafetyBox;

public class UpdateSafetyBoxCommandHandler : IRequestHandler<UpdateSafetyBoxCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateSafetyBoxCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateSafetyBoxCommand request, CancellationToken cancellationToken)
    {
        var safetyBox = await _context.SafetyBoxes
            .Include(s => s.Shelves)
            .Include(s => s.Properties)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (safetyBox == null)
        {
            throw new NotFoundException(nameof(SafetyBox), request.Id);
        }

        var location = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.LocationId && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            return Result.Failure("Location not found.");
        }

        var existingBox = await _context.SafetyBoxes
            .AnyAsync(s => s.BoxNumber == request.BoxNumber && s.Id != request.Id && !s.IsDeleted, cancellationToken);

        if (existingBox)
        {
            return Result.Failure($"Safety box with number '{request.BoxNumber}' already exists.");
        }

        if (request.TotalShelves < safetyBox.Shelves?.Count(s => s.Properties?.Any() == true))
        {
            return Result.Failure("Cannot reduce total shelves below number of occupied shelves.");
        }

        var oldSafetyBox = new SafetyBox(safetyBox.BoxNumber, safetyBox.TotalShelves, safetyBox.LocationId);

        typeof(SafetyBox).GetProperty(nameof(SafetyBox.BoxNumber))?.SetValue(safetyBox, request.BoxNumber);
        typeof(SafetyBox).GetProperty(nameof(SafetyBox.TotalShelves))?.SetValue(safetyBox, request.TotalShelves);
        typeof(SafetyBox).GetProperty(nameof(SafetyBox.LocationId))?.SetValue(safetyBox, request.LocationId);

        safetyBox.MarkUpdated();

        if (request.TotalShelves > safetyBox.Shelves?.Count)
        {
            for (int i = safetyBox.Shelves.Count + 1; i <= request.TotalShelves; i++)
            {
                var shelf = new SafetyBoxShelf(safetyBox.Id, i);
                _context.SafetyBoxShelves.Add(shelf);
            }
        }

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(SafetyBox),
            safetyBox.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<SafetyBox>(safetyBox, oldSafetyBox, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}