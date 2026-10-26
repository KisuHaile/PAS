using Application.Common.Security;
using Application.Events;
using MediatR;

namespace Application.Features.Receiving.Suppliers.Commands.DeleteSupplier;

[Authorize(Permissions = Permissions.Suppliers.Delete)]
public record DeleteSupplierCommand(Guid Id) : IRequest<Result>;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteSupplierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .Include(s => s.ReceivingNotes)
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (supplier == null)
        {
            throw new NotFoundException(nameof(Domain.Receiving.Supplier), request.Id);
        }

        // Check if supplier has associated receiving notes
        var hasReceivingNotes = await _context.ReceivingNotes
            .AnyAsync(r => r.SupplierId == request.Id && !r.IsDeleted, cancellationToken);

        if (hasReceivingNotes)
        {
            return Result.Failure("Cannot delete supplier with existing receiving notes. Deactivate instead.");
        }

        supplier.SoftDelete();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "DELETE",
            nameof(Domain.Receiving.Supplier),
            supplier.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Domain.Receiving.Supplier>(supplier, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}