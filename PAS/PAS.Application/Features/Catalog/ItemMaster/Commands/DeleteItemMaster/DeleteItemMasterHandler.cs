using Application.Events;
using MediatR;

namespace Application.Features.Catalog.ItemMasters.Commands.DeleteItemMaster;

public class DeleteItemMasterCommandHandler : IRequestHandler<DeleteItemMasterCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteItemMasterCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteItemMasterCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.ItemMasters
            .Include(i => i.InventoryStocks)
            .Include(i => i.ServiceRequestDetails)
            .FirstOrDefaultAsync(i => i.Id == request.Id && !i.IsDeleted, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(ItemMaster), request.Id);
        }

        if (item.InventoryStocks?.Any(s => s.CurrentQuantity > 0) == true)
        {
            return Result.Failure("Cannot delete item with existing stock.");
        }

        if (item.ServiceRequestDetails?.Any() == true)
        {
            return Result.Failure("Cannot delete item with existing service requests.");
        }

        item.SoftDelete();

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(ItemMaster),
            item.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<ItemMaster>(item, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}