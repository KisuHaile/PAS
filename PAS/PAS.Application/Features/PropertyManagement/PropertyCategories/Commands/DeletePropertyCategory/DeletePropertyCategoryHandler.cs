using Application.Events;
using MediatR;

namespace Application.Features.PropertyManagement.PropertyCategories.Commands.DeletePropertyCategory;

public class DeletePropertyCategoryCommandHandler : IRequestHandler<DeletePropertyCategoryCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeletePropertyCategoryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeletePropertyCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.PropertyCategories
            .Include(c => c.Properties)
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(nameof(PropertyCategory), request.Id);
        }

        if (category.Properties?.Any(p => !p.IsDeleted) == true)
        {
            return Result.Failure("Cannot delete category with existing properties.");
        }

        category.SoftDelete();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(PropertyCategory),
            category.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<PropertyCategory>(category, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}