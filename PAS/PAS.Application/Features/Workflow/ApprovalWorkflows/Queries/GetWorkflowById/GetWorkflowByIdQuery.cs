using Application.Common.Security;
using Application.Features.Workflow.ApprovalWorkflows.Dtos;
using MediatR;

namespace Application.Features.Workflow.ApprovalWorkflows.Queries.GetWorkflowById;

[Authorize(Permissions = Permissions.Workflow.View)]
public record GetWorkflowByIdQuery(Guid Id) : IRequest<Result<ApprovalWorkflowDetailDto>>;