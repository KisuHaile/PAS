using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.SafetyBoxes.Commands.DeleteSafetyBox;

public class DeleteSafetyBoxCommandHandler : IRequestHandler<DeleteSafetyBoxCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteSafetyBoxCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteSafetyBoxCommand request, CancellationToken cancellationToken)
    {
        var safetyBox = await _context.SafetyBoxes
            .Include(s => s.Properties)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (safetyBox == null)
        {
            throw new NotFoundException(nameof(SafetyBox), request.Id);
        }

        if (safetyBox.Properties?.Any(p => !p.IsDeleted) == true)
        {
            return Result.Failure("Cannot delete safety box with existing properties.");
        }

        safetyBox.SoftDelete();

        var shelves = await _context.SafetyBoxShelves
            .Where(s => s.SafetyBoxId == request.Id)
            .ToListAsync(cancellationToken);

        foreach (var shelf in shelves)
        {
            shelf.SoftDelete();
        }

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(SafetyBox),
            safetyBox.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<SafetyBox>(safetyBox, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}