using Application.Common.Security;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.UpdateWorkflow;

[Authorize(Permissions = Permissions.Workflow.Edit)]
public record UpdateWorkflowCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string WorkflowName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}