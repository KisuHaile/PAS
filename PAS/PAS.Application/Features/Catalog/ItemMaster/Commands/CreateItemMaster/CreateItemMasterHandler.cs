using Application.Events;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Commands.CreateItemMaster;

public class CreateItemMasterCommandHandler : IRequestHandler<CreateItemMasterCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateItemMasterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateItemMasterCommand request, CancellationToken cancellationToken)
    {
        var existingSku = await _context.ItemMasters
            .AnyAsync(i => i.SKU == request.SKU && !i.IsDeleted, cancellationToken);

        if (existingSku)
        {
            return Result<Guid>.Failure($"SKU '{request.SKU}' already exists.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            return Result<Guid>.Failure("Category not found.");
        }

        var item = new ItemMaster(
            request.SKU,
            request.ItemName,
            request.CategoryId,
            request.UnitOfMeasure,
            request.RequiresInspection,
            request.MinStockLevel,
            request.UnitPrice,
            request.Status);

        _context.ItemMasters.Add(item);
        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(ItemMaster),
            item.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<ItemMaster>(item, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}