using Application.Events;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.CreateWorkflow;

public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateWorkflowCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result<Guid>> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
    {
        var existingName = await _context.ApprovalWorkflows
            .AnyAsync(w => w.WorkflowName == request.WorkflowName && !w.IsDeleted, cancellationToken);

        if (existingName)
        {
            return Result<Guid>.Failure($"Workflow with name '{request.WorkflowName}' already exists.");
        }

        var workflow = new ApprovalWorkflow(request.WorkflowName, request.Description);

        _context.ApprovalWorkflows.Add(workflow);
        await _context.SaveChangesAsync(cancellationToken);

        if (_currentUser.UserGuid.HasValue)
        {
            var auditTrail = new AuditTrail(
                _currentUser.UserGuid.Value,
                "CREATE",
                nameof(ApprovalWorkflow),
                workflow.Id);
            _context.AuditTrails.Add(auditTrail);
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _mediator.Publish(new EntityCreatedEvent<ApprovalWorkflow>(workflow, _currentUser.UserGuid), cancellationToken);

        return Result<Guid>.Success(workflow.Id);
    }
}