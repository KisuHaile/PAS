using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.UpdatePropertyCategory;

public class UpdatePropertyCategoryCommandHandler : IRequestHandler<UpdatePropertyCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdatePropertyCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdatePropertyCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.PropertyCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(PropertyCategory), request.Id);
        }

        var existingName = await _context.PropertyCategories
            .AnyAsync(c => c.Name == request.Name && c.Id != request.Id && !c.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result.Failure($"Property category with name '{request.Name}' already exists.");
        }

        var oldCategory = new PropertyCategory(category.Name, category.Description);

        typeof(PropertyCategory).GetProperty(nameof(PropertyCategory.Name))?.SetValue(category, request.Name);
        typeof(PropertyCategory).GetProperty(nameof(PropertyCategory.Description))?.SetValue(category, request.Description);

        category.MarkUpdated();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(PropertyCategory),
            category.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<PropertyCategory>(category, oldCategory, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}