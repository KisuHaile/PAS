using Application.Events;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.UpdateWorkflow;

public class UpdateWorkflowCommandHandler : IRequestHandler<UpdateWorkflowCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateWorkflowCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(UpdateWorkflowCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _context.ApprovalWorkflows
            .FirstOrDefaultAsync(w => w.Id == request.Id && !w.IsDeleted, cancellationToken);

        if (workflow == null)
        {
            throw new NotFoundException(nameof(ApprovalWorkflow), request.Id);
        }

        var existingName = await _context.ApprovalWorkflows
            .AnyAsync(w => w.WorkflowName == request.WorkflowName && w.Id != request.Id && !w.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result.Failure($"Workflow with name '{request.WorkflowName}' already exists.");
        }

        var oldWorkflow = new ApprovalWorkflow(workflow.WorkflowName, workflow.Description);

        typeof(ApprovalWorkflow).GetProperty(nameof(ApprovalWorkflow.WorkflowName))?.SetValue(workflow, request.WorkflowName);
        typeof(ApprovalWorkflow).GetProperty(nameof(ApprovalWorkflow.Description))?.SetValue(workflow, request.Description);

        workflow.MarkUpdated();

        if (_currentUser.UserGuid.HasValue)
        {
            var auditTrail = new AuditTrail(
                _currentUser.UserGuid.Value,
                "UPDATE",
                nameof(ApprovalWorkflow),
                workflow.Id);
            _context.AuditTrails.Add(auditTrail);
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new EntityUpdatedEvent<ApprovalWorkflow>(workflow, oldWorkflow, _currentUser.UserGuid), cancellationToken);

        return Result.Success();
    }
}