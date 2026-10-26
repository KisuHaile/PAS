using Application.Common.Security;
using Application.Features.Workflow.ApprovalWorkflows.Dtos;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Queries.GetWorkflows;

[Authorize(Permissions = Permissions.Workflow.View)]
public record GetWorkflowsQuery : IRequest<Result<List<ApprovalWorkflowDto>>>
{
    public string? SearchTerm { get; init; }
}