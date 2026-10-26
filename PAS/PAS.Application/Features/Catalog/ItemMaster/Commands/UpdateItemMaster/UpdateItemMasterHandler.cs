using Application.Events;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Commands.UpdateItemMaster;

public class UpdateItemMasterCommandHandler : IRequestHandler<UpdateItemMasterCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateItemMasterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateItemMasterCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ItemMasters
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ItemMaster), request.Id);
        }

        var existingSku = await _context.ItemMasters
            .AnyAsync(i => i.SKU == request.SKU && i.Id != request.Id && !i.IsDeleted, cancellationToken);

        if (existingSku)
        {
            return Result.Failure($"SKU '{request.SKU}' already exists.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId && !c.IsDeleted, cancellationToken);

        if (category == null)
        {
            return Result.Failure("Category not found.");
        }

        var oldItem = new ItemMaster(
            item.SKU,
            item.ItemName,
            item.CategoryId,
            item.UnitOfMeasure,
            item.RequiresInspection,
            item.MinStockLevel);

        typeof(ItemMaster).GetProperty(nameof(ItemMaster.SKU))?.SetValue(item, request.SKU);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.ItemName))?.SetValue(item, request.ItemName);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.CategoryId))?.SetValue(item, request.CategoryId);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.UnitOfMeasure))?.SetValue(item, request.UnitOfMeasure);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.RequiresInspection))?.SetValue(item, request.RequiresInspection);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.MinStockLevel))?.SetValue(item, request.MinStockLevel);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.UnitPrice))?.SetValue(item, request.UnitPrice);
        typeof(ItemMaster).GetProperty(nameof(ItemMaster.Status))?.SetValue(item, request.Status);

        item.MarkUpdated();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(ItemMaster),
            item.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<ItemMaster>(item, oldItem, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}