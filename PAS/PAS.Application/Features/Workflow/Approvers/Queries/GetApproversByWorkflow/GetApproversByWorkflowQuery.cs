using Application.Common.Security;
using Application.Features.Workflow.Approvers.Dtos;
using MediatR;

namespace Application.Features.Workflow.Approvers.Queries.GetApproversByWorkflow;

[Authorize(Permissions = Permissions.Workflow.View)]
public record GetApproversByWorkflowQuery(Guid WorkflowId) : IRequest<Result<List<ApproverDto>>>;