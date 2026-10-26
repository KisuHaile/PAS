using MediatR;

namespace Application.Features.Receiving.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateSupplierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var existingTin = await _context.Suppliers
            .AnyAsync(s => s.TinNumber == request.TinNumber && !s.IsDeleted, cancellationToken);

        if (existingTin)
        {
            return Result<Guid>.Failure($"Supplier with TIN '{request.TinNumber}' already exists.");
        }

        var existingEmail = await _context.Suppliers
            .AnyAsync(s => s.Email == request.Email && !s.IsDeleted, cancellationToken);

        if (existingEmail && !string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<Guid>.Failure($"Supplier with email '{request.Email}' already exists.");
        }

        var supplier = new Domain.Receiving.Supplier(
            request.SupplierName,
            request.ContactPerson,
            request.TinNumber);

        // Set additional properties using reflection if properties exist
        var emailProperty = typeof(Domain.Receiving.Supplier).GetProperty("Email");
        if (emailProperty != null && !string.IsNullOrWhiteSpace(request.Email))
        {
            emailProperty.SetValue(supplier, request.Email);
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

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        // Create contacts if the property exists
        var contactsProperty = typeof(Domain.Receiving.Supplier).GetProperty("Contacts");
        if (contactsProperty != null && request.Contacts.Any())
        {
            // This would require a Contacts entity to be implemented
            // For now, we'll skip or log
        }

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(Domain.Receiving.Supplier),
            supplier.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Receiving.Supplier>(supplier, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(supplier.Id);
    }
}