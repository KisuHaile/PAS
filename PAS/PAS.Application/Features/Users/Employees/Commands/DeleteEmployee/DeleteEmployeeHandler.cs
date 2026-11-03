using MediatR;

namespace Application.Features.Users.Employees.Commands;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteEmployeeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
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

        // Check if employee has associated records
        var hasActivity = await _context.AuditTrails
            .AnyAsync(a => a.UserId == employee.Id, cancellationToken);

        if (hasActivity)
        {
            return Result.Failure("Cannot delete employee with system activity. Consider deactivating instead.");
        }

        employee.SoftDelete();

        // Create audit trail
        var auditTrail = new AuditTrail(
            _currentUser.UserGuid.Value,
            "DELETE",
            nameof(Domain.Users.Employee),
            employee.Id);
        _context.AuditTrails.Add(auditTrail);

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Domain.Users.Employee>(employee, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}
