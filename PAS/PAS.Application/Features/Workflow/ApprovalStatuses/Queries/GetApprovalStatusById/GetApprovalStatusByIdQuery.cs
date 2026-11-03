using Application.Common.Security;
using Application.Features.Workflow.ApprovalStatuses.Dtos;
using MediatR;

namespace Application.Features.Workflow.ApprovalStatuses.Queries.GetApprovalStatusById;

[Authorize(Permissions = Permissions.Workflow.View)]
public record GetApprovalStatusByIdQuery(Guid Id) : IRequest<Result<ApprovalStatusDto>>;