using Application.Common.Security;
using MediatR;

namespace Application.Features.Workflow.Approvers.Commands.AssignApprover;

public record AssignApproverCommand : IRequest<Result<Guid>>
{
    public Guid WorkflowId { get; init; }
    public Guid UserId { get; init; }
    public int ApprovalLevel { get; init; }
}