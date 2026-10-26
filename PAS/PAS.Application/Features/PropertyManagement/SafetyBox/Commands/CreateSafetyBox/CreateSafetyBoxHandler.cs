using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.CreateSafetyBox;

public class CreateSafetyBoxCommandHandler : IRequestHandler<CreateSafetyBoxCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateSafetyBoxCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateSafetyBoxCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.LocationId && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            return Result<Guid>.Failure("Location not found.");
        }

        var existingBox = await _context.SafetyBoxes
            .AnyAsync(s => s.BoxNumber == request.BoxNumber && !s.IsDeleted, cancellationToken);

        if (existingBox)
        {
            return Result<Guid>.Failure($"Safety box with number '{request.BoxNumber}' already exists.");
        }

        var safetyBox = new SafetyBox(request.BoxNumber, request.TotalShelves, request.LocationId);

        _context.SafetyBoxes.Add(safetyBox);

        for (int i = 1; i <= request.TotalShelves; i++)
        {
            var shelf = new SafetyBoxShelf(safetyBox.Id, i);
            _context.SafetyBoxShelves.Add(shelf);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(SafetyBox),
            safetyBox.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<SafetyBox>(safetyBox, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(safetyBox.Id);
    }
}