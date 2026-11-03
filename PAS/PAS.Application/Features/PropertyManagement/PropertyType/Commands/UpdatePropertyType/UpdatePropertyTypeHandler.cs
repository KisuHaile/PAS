using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.UpdatePropertyType;

public class UpdatePropertyTypeCommandHandler : IRequestHandler<UpdatePropertyTypeCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdatePropertyTypeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdatePropertyTypeCommand request, CancellationToken cancellationToken)
    {
        var propertyType = await _context.PropertyTypes
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (propertyType == null)
        {
            throw new NotFoundException(nameof(PropertyType), request.Id);
        }

        var existingName = await _context.PropertyTypes
            .AnyAsync(p => p.Name == request.Name && p.Id != request.Id && !p.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result.Failure($"Property type with name '{request.Name}' already exists.");
        }

        var oldPropertyType = new PropertyType(propertyType.Name, propertyType.Description);

        typeof(PropertyType).GetProperty(nameof(PropertyType.Name))?.SetValue(propertyType, request.Name);
        typeof(PropertyType).GetProperty(nameof(PropertyType.Description))?.SetValue(propertyType, request.Description);

        propertyType.MarkUpdated();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(PropertyType),
            propertyType.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<PropertyType>(propertyType, oldPropertyType, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}