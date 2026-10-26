using Application.Events;
using MediatR;

namespace Application.Features.Workflow.ApprovalStatuses.Commands.CreateApprovalStatus;

public class CreateApprovalStatusCommandHandler : IRequestHandler<CreateApprovalStatusCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateApprovalStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateApprovalStatusCommand request, CancellationToken cancellationToken)
    {
        var existingName = await _context.ApprovalStatuses
            .AnyAsync(s => s.StatusName == request.StatusName && !s.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Approval status with name '{request.StatusName}' already exists.");
        }

        var status = new ApprovalStatus(request.StatusName);

        _context.ApprovalStatuses.Add(status);
        await _context.SaveChangesAsync(cancellationToken);

        var auditTrail = new AuditTrail(
            _currentUser.UserGuid ?? Guid.Empty,
            "CREATE",
            nameof(ApprovalStatus),
            status.Id);
        _context.AuditTrails.Add(auditTrail);
        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityCreatedEvent<ApprovalStatus>(status, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(status.Id);
    }
}