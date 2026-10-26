using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.CreatePropertyType;

public class CreatePropertyTypeCommandHandler : IRequestHandler<CreatePropertyTypeCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreatePropertyTypeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
    {
        var existingName = await _context.PropertyTypes
            .AnyAsync(p => p.Name == request.Name && !p.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Property type with name '{request.Name}' already exists.");
        }

        var propertyType = new PropertyType(request.Name, request.Description);

        _context.PropertyTypes.Add(propertyType);
        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(PropertyType),
            propertyType.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<PropertyType>(propertyType, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(propertyType.Id);
    }
}