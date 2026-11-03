using Application.Events;
using MediatR;

namespace Application.Features.Workflow.Approvers.Commands.RemoveApprover;

public class RemoveApproverCommandHandler : IRequestHandler<RemoveApproverCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public RemoveApproverCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(RemoveApproverCommand request, CancellationToken cancellationToken)
    {
        var approver = await _context.Approvers
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken);

        if (approver == null)
        {
            throw new NotFoundException(nameof(Approver), request.Id);
        }

        approver.SoftDelete();

        if (_currentUser.UserGuid.HasValue)
        {
            var auditTrail = new AuditTrail(
                _currentUser.UserGuid.Value,
                "REMOVE_APPROVER",
                nameof(Approver),
                approver.Id);
            _context.AuditTrails.Add(auditTrail);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<Approver>(approver, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}