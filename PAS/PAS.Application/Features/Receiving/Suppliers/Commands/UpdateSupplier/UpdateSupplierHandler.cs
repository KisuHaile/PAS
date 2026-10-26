using MediatR;

namespace Application.Features.Receiving.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateSupplierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, cancellationToken);

        if (supplier == null)
        {
            throw new NotFoundException(nameof(Domain.Receiving.Supplier), request.Id);
        }

        var existingTin = await _context.Suppliers
            .AnyAsync(s => s.TinNumber == request.TinNumber && s.Id != request.Id && !s.IsDeleted, cancellationToken);

        if (existingTin)
        {
            return Result.Failure($"Supplier with TIN '{request.TinNumber}' already exists.");
        }

        var existingEmail = await _context.Suppliers
            .AnyAsync(s => s.Email == request.Email && s.Id != request.Id && !s.IsDeleted, cancellationToken);

        if (existingEmail && !string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure($"Supplier with email '{request.Email}' already exists.");
        }

        var oldSupplier = new Domain.Receiving.Supplier(
            supplier.SupplierName,
            supplier.ContactPerson,
            supplier.TinNumber);

        // Update properties
        typeof(Domain.Receiving.Supplier).GetProperty(nameof(Domain.Receiving.Supplier.SupplierName))?.SetValue(supplier, request.SupplierName);
        typeof(Domain.Receiving.Supplier).GetProperty(nameof(Domain.Receiving.Supplier.ContactPerson))?.SetValue(supplier, request.ContactPerson);
        typeof(Domain.Receiving.Supplier).GetProperty(nameof(Domain.Receiving.Supplier.TinNumber))?.SetValue(supplier, request.TinNumber);

        var emailProperty = typeof(Domain.Receiving.Supplier).GetProperty("Email");
        if (emailProperty != null)
        {
            emailProperty.SetValue(supplier, request.Email ?? string.Empty);
        }

        var phoneProperty = typeof(Domain.Receiving.Supplier).GetProperty("Phone");
        if (phoneProperty != null)
        {
            phoneProperty.SetValue(supplier, request.Phone ?? string.Empty);
        }

        var addressProperty = typeof(Domain.Receiving.Supplier).GetProperty("Address");
        if (addressProperty != null)
        {
            addressProperty.SetValue(supplier, request.Address ?? string.Empty);
        }

        var isActiveProperty = typeof(Domain.Receiving.Supplier).GetProperty("IsActive");
        if (isActiveProperty != null)
        {
            isActiveProperty.SetValue(supplier, request.IsActive);
        }

        supplier.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "UPDATE",
            nameof(Domain.Receiving.Supplier),
            supplier.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Receiving.Supplier>(supplier, oldSupplier, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}