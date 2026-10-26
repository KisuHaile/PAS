using Application.Events;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.DeleteWorkflow;

public class DeleteWorkflowCommandHandler : IRequestHandler<DeleteWorkflowCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public DeleteWorkflowCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _context.ApprovalWorkflows
            .Include(w => w.Approvers)
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (workflow == null)
        {
            throw new NotFoundException(nameof(ApprovalWorkflow), request.Id);
        }

        workflow.SoftDelete();

        if (workflow.Approvers?.Any() == true)
        {
            foreach (var approver in workflow.Approvers.Where(a => !a.IsDeleted))
            {
                approver.SoftDelete();
            }
        }

        if (_currentUser.UserGuid.HasValue)
        {
            var auditTrail = new AuditTrail(
                _currentUser.UserGuid.Value,
                "DELETE",
                nameof(ApprovalWorkflow),
                workflow.Id);
            _context.AuditTrails.Add(auditTrail);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityDeletedEvent<ApprovalWorkflow>(workflow, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}