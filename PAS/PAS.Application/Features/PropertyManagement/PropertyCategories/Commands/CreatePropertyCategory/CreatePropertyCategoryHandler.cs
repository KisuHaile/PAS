using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.CreatePropertyCategory;

public class CreatePropertyCategoryCommandHandler : IRequestHandler<CreatePropertyCategoryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreatePropertyCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreatePropertyCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingName = await _context.PropertyCategories
            .AnyAsync(c => c.Name == request.Name && !c.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Property category with name '{request.Name}' already exists.");
        }

        var category = new PropertyCategory(request.Name, request.Description);

        _context.PropertyCategories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(PropertyCategory),
            category.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<PropertyCategory>(category, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(category.Id);
    }
}