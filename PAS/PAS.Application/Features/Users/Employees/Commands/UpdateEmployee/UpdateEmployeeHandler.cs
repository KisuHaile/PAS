namespace Application.Features.Users.Employees.Commands;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateEmployeeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.UserGuid.HasValue)
        {
            return Result.Failure("User not authenticated.");
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

        if (employee == null)
        {
            throw new NotFoundException(nameof(Domain.Users.Employee), request.Id);
        }

        // Check for duplicate employee code
        var existingCode = await _context.Employees
            .AnyAsync(e => e.EmployeeCode == request.EmployeeCode && e.Id != request.Id && !e.IsDeleted, cancellationToken);

        if (existingCode)
        {
            return Result.Failure($"Employee with code '{request.EmployeeCode}' already exists.");
        }

        // Check for duplicate email if provided
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var existingEmail = await _context.Employees
                .AnyAsync(e => e.Email == request.Email && e.Id != request.Id && !e.IsDeleted, cancellationToken);

            if (existingEmail)
            {
                return Result.Failure($"Employee with email '{request.Email}' already exists.");
            }
        }

        // Create a copy for event
        var oldEmployee = new Domain.Users.Employee(
            employee.EmployeeCode,
            employee.FullName,
            employee.Department);

        // Update properties
        typeof(Domain.Users.Employee).GetProperty(nameof(Domain.Users.Employee.EmployeeCode))?.SetValue(employee, request.EmployeeCode);
        typeof(Domain.Users.Employee).GetProperty(nameof(Domain.Users.Employee.FullName))?.SetValue(employee, request.FullName);
        typeof(Domain.Users.Employee).GetProperty(nameof(Domain.Users.Employee.Department))?.SetValue(employee, request.Department);
        typeof(Domain.Users.Employee).GetProperty("Position")?.SetValue(employee, request.Position);
        typeof(Domain.Users.Employee).GetProperty("Email")?.SetValue(employee, request.Email);
        typeof(Domain.Users.Employee).GetProperty("Phone")?.SetValue(employee, request.Phone);
        typeof(Domain.Users.Employee).GetProperty("HireDate")?.SetValue(employee, request.HireDate);
        typeof(Domain.Users.Employee).GetProperty("IsActive")?.SetValue(employee, request.IsActive);

        employee.MarkUpdated();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "UPDATE",
            nameof(Domain.Users.Employee),
            employee.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<Domain.Users.Employee>(employee, oldEmployee, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}