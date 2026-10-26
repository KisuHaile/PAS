using Application.Common.Security;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Commands.CreateWorkflow;

[Authorize(Permissions = Permissions.Workflow.Create)]
public record CreateWorkflowCommand : IRequest<Result<Guid>>
{
    public string WorkflowName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}