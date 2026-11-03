using Application.Events;
using MediatR;

namespace Application.Features.Catalog.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existingName = await _context.Categories
            .AnyAsync(c => c.Name == request.Name && !c.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Category with name '{request.Name}' already exists.");
        }

        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _context.Categories
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value && !c.IsDeleted, cancellationToken);

            if (!parentExists)
            {
                return Result<Guid>.Failure("Parent category not found.");
            }
        }

        var category = new Category(request.Name, request.Description, request.ParentCategoryId);

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        if (_currentUser.UserGuid.HasValue)
        {
            var auditTrail = new AuditTrail(
                _currentUser.UserGuid.Value,
                "CREATE",
                nameof(Category),
                category.Id);
            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _mediator.Publish(new EntityCreatedEvent<Category>(category, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(category.Id);
    }
}