using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.Properties.Commands.UpdateProperty;

public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdatePropertyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (property == null)
        {
            throw new NotFoundException(nameof(Domain.PropertyManagement.Property), request.Id);
        }

        var existingTag = await _context.Properties
            .AnyAsync(p => p.TagNumber == request.TagNumber && p.Id != request.Id && !p.IsDeleted, cancellationToken);

        if (existingTag)
        {
            return Result.Failure($"Property with tag number '{request.TagNumber}' already exists.");
        }

        var propertyType = await _context.PropertyTypes
            .FirstOrDefaultAsync(p => p.Id == request.PropertyTypeId && !p.IsDeleted, cancellationToken);

        if (propertyType == null)
        {
            return Result.Failure("Property type not found.");
        }

        if (request.PropertyCategoryId.HasValue)
        {
            var category = await _context.PropertyCategories
                .FirstOrDefaultAsync(c => c.Id == request.PropertyCategoryId && !c.IsDeleted, cancellationToken);

            if (category == null)
            {
                return Result.Failure("Property category not found.");
            }
        }

        var location = await _context.PropertyLocations
            .FirstOrDefaultAsync(l => l.Id == request.LocationId && !l.IsDeleted, cancellationToken);

        if (location == null)
        {
            return Result.Failure("Location not found.");
        }

        if (request.SafetyBoxId.HasValue)
        {
            var safetyBox = await _context.SafetyBoxes
                .Include(s => s.Shelves)
                .FirstOrDefaultAsync(s => s.Id == request.SafetyBoxId && !s.IsDeleted, cancellationToken);

            if (safetyBox == null)
            {
                return Result.Failure("Safety box not found.");
            }

            if (safetyBox.LocationId != request.LocationId)
            {
                return Result.Failure("Safety box does not belong to the selected location.");
            }
        }

        var oldProperty = new Domain.PropertyManagement.Property(
            property.TagNumber,
            property.Name,
            property.SerialNumber,
            property.PropertyTypeId,
            property.UnitPrice,
            property.Quantity,
            property.PurchaseDate,
            property.LocationId,
            property.SafetyBoxId);

        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.TagNumber))?.SetValue(property, request.TagNumber);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.Name))?.SetValue(property, request.Name);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.SerialNumber))?.SetValue(property, request.SerialNumber);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.PropertyTypeId))?.SetValue(property, request.PropertyTypeId);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.PropertyCategoryId))?.SetValue(property, request.PropertyCategoryId);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.UnitPrice))?.SetValue(property, request.UnitPrice);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.Quantity))?.SetValue(property, request.Quantity);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.PurchaseDate))?.SetValue(property, request.PurchaseDate);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.LocationId))?.SetValue(property, request.LocationId);
        typeof(Domain.PropertyManagement.Property).GetProperty(nameof(Domain.PropertyManagement.Property.SafetyBoxId))?.SetValue(property, request.SafetyBoxId);

        property.MarkUpdated();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(Domain.PropertyManagement.Property),
            property.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.PropertyManagement.Property>(property, oldProperty, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}