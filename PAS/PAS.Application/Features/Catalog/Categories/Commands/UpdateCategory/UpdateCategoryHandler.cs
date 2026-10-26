using Application.Events;
using MediatR;

namespace Application.Features.Catalog.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        if (request.ParentCategoryId.HasValue && request.ParentCategoryId.Value == request.Id)
        {
            return Result.Failure("Category cannot be its own parent.");
        }

        var existingName = await _context.Categories
            .AnyAsync(c => c.Name == request.Name && c.Id != request.Id && !c.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result.Failure($"Category with name '{request.Name}' already exists.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _context.Categories
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value && !c.IsDeleted, cancellationToken);

            if (!parentExists)
            {
                return Result.Failure("Parent category not found.");
            }

            // Check for circular reference
            var isCircular = await CheckCircularReference(request.Id, request.ParentCategoryId.Value, cancellationToken);
            if (isCircular)
            {
                return Result.Failure("Circular reference detected. A category cannot be its own ancestor.");
            }
        }

        var oldCategory = new Category(category.Name, category.Description, category.ParentCategoryId);

        typeof(Category).GetProperty(nameof(Category.Name))?.SetValue(category, request.Name);
        typeof(Category).GetProperty(nameof(Category.Description))?.SetValue(category, request.Description);
        typeof(Category).GetProperty(nameof(Category.ParentCategoryId))?.SetValue(category, request.ParentCategoryId);

        category.MarkUpdated();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(Category),
            category.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Category>(category, oldCategory, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }

    private async Task<bool> CheckCircularReference(Guid categoryId, Guid parentId, CancellationToken cancellationToken)
    {
        var current = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == parentId, cancellationToken);

        while (current != null && current.ParentCategoryId.HasValue)
        {
            if (current.ParentCategoryId.Value == categoryId)
                return true;

            current = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == current.ParentCategoryId.Value, cancellationToken);
        }

        return false;
    }
}