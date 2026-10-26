using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyTypes.Commands.DeletePropertyType;

public class DeletePropertyTypeCommandHandler : IRequestHandler<DeletePropertyTypeCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeletePropertyTypeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
    {
        var propertyType = await _context.PropertyTypes
            .Include(p => p.Properties)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, cancellationToken);

        if (propertyType == null)
        {
            throw new NotFoundException(nameof(PropertyType), request.Id);
        }

        if (propertyType.Properties?.Any(p => !p.IsDeleted) == true)
        {
            return Result.Failure("Cannot delete property type with existing properties.");
        }

        propertyType.SoftDelete();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(PropertyType),
            propertyType.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<PropertyType>(propertyType, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}