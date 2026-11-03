namespace Application.Features.Users.Employees.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateEmployeeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result<Guid>.Failure("User not authenticated.");
        }

        // Check for duplicate employee code
        var existingCode = await _context.Employees
            .AnyAsync(e => e.EmployeeCode == request.EmployeeCode && !e.IsDeleted, cancellationToken);

        if (existingCode)
        {
            return Result<Guid>.Failure($"Employee with code '{request.EmployeeCode}' already exists.");
        }

        // Check for duplicate email if provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingEmail = await _context.Employees
                .AnyAsync(e => e.Email == request.Email && !e.IsDeleted, cancellationToken);

            if (existingEmail)
            {
                return Result<Guid>.Failure($"Employee with email '{request.Email}' already exists.");
            }
        }

        var employee = new Domain.Users.Employee(
            request.EmployeeCode,
            request.FullName,
            request.Department);

        // Set additional properties
        if (!string.IsNullOrWhiteSpace(request.Position))
            typeof(Domain.Users.Employee).GetProperty("Position")?.SetValue(employee, request.Position);

        if (!string.IsNullOrWhiteSpace(request.Email))
            typeof(Domain.Users.Employee).GetProperty("Email")?.SetValue(employee, request.Email);

        if (!string.IsNullOrWhiteSpace(request.Phone))
            typeof(Domain.Users.Employee).GetProperty("Phone")?.SetValue(employee, request.Phone);

        if (request.HireDate.HasValue)
            typeof(Domain.Users.Employee).GetProperty("HireDate")?.SetValue(employee, request.HireDate);

        typeof(Domain.Users.Employee).GetProperty("IsActive")?.SetValue(employee, true);

        _context.Employees.Add(employee);

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "CREATE",
            nameof(Domain.Users.Employee),
            employee.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<Domain.Users.Employee>(employee, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(employee.Id);
    }
}