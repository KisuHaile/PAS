using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.TransferProperty;

public class TransferPropertyCommandHandler : IRequestHandler<TransferPropertyCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TransferPropertyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(TransferPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .Include(p => p.Location)
            .Include(p => p.SafetyBox)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (property == null)
        {
            throw new NotFoundException(nameof(Domain.PropertyManagement.Property), request.Id);
        }

        var newLocation = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.NewLocationId && !l.IsDeleted, cancellationToken);

        if (newLocation == null)
        {
            return Result.Failure("New location not found.");
        }

        if (request.NewSafetyBoxId.HasValue)
        {
            var safetyBox = await _context.SafetyBoxes
                .Include(s => s.Shelves)
                .FirstOrDefaultAsync(s => s.Id == request.NewSafetyBoxId && !s.IsDeleted, cancellationToken);

            if (safetyBox == null)
            {
                return Result.Failure("Safety box not found.");
            }

            if (safetyBox.LocationId != request.NewLocationId)
            {
                return Result.Failure("Safety box does not belong to the selected location.");
            }

            if (request.NewShelfNumber.HasValue)
            {
                var shelf = safetyBox.Shelves?.FirstOrDefault(sh => sh.ShelfNumber == request.NewShelfNumber);
                if (shelf == null)
                {
                    return Result.Failure($"Shelf number {request.NewShelfNumber} not found in safety box.");
                }
            }
        }

        var oldLocationId = property.LocationId;
        var oldSafetyBoxId = property.SafetyBoxId;

        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.LocationId))?.SetValue(property, request.NewLocationId);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.SafetyBoxId))?.SetValue(property, request.NewSafetyBoxId);
        property.MarkUpdated();

        var transferRecord = new TransferRecord(
            property.Id,
            oldLocationId,
            request.NewLocationId,
            property.Quantity);

        _context.TransferRecords.Add(transferRecord);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "TRANSFER",
            nameof(Domain.PropertyManagement.Property),
            property.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}