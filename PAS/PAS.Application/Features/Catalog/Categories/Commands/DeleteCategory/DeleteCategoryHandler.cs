using Application.Events;
using MediatR;

namespace Application.Features.Catalog.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        if (category.SubCategories.Any(s => !s.IsDeleted))
        {
            return Result.Failure("Cannot delete category with existing subcategories.");
        }

        if (category.Items.Any(i => !i.IsDeleted))
        {
            return Result.Failure("Cannot delete category with existing items.");
        }

        category.SoftDelete();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(Category),
            category.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Category>(category, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}